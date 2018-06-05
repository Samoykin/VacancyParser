namespace VacancyParser.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.IO;
    using System.Linq;
    using Model;
    using NLog;

    /// <summary>База банных.</summary>
    public class DBConnect
    {        
        private const string DatabaseName = "VacancyDB.sqlite";
        private const string Pass = "Xt,ehfirf3";
        private Logger logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>Создать БД.</summary>
        public void CreateBase()
        {
            if (!File.Exists(DatabaseName))
            {
                SQLiteConnection.CreateFile(DatabaseName);
                var conn = new SQLiteConnection("Data Source=DBTels.sqlite;Version=3;");
                conn.SetPassword(Pass);

                this.logger.Info("Создана БД " + DatabaseName);
            }
        }

        /// <summary>Создать таблицу.</summary>
        public void CreateTable()
        {
            
            using (var conn = new SQLiteConnection(this.Connstring()))
            {
                var query = "CREATE TABLE vacancyInfo (id INTEGER PRIMARY KEY UNIQUE, VacId VARCHAR, Name VARCHAR, Company VARCHAR, Website VARCHAR, Salary VARCHAR, Exp VARCHAR, City VARCHAR, Description VARCHAR, Address VARCHAR, Type VARCHAR, DateVac VARCHAR);";

                SQLiteCommand command = new SQLiteCommand(query, conn);
                conn.Open();
                command.ExecuteNonQuery();
                conn.Close();
            }
        }

        /// <summary>Записать в БД.</summary>
        /// <param name="vacancies">Вакансии.</param>
        public void WriteData(List<VacModel> vacancies)
        {
                using (var conn = new SQLiteConnection(this.Connstring()))
                {
                    for (int i = 0; i < vacancies.Count(); i++)
                    {
                        string strDescr = string.Join(" ", vacancies[i].Description.ToArray());

                        var command = new SQLiteCommand($"INSERT INTO 'vacancyInfo' ('VacId', 'Name', 'Company', 'Website', 'Salary', 'Exp', 'City', 'Description', 'Address', 'Type', 'DateVac') VALUES ('{vacancies[i].Id}', '{vacancies[i].Name}', '{vacancies[i].Company}', '{vacancies[i].Website}', '{vacancies[i].Salary}', '{vacancies[i].Exp}', '{vacancies[i].City}', '{strDescr}', '{vacancies[i].Address}', '{vacancies[i].Type}', '{vacancies[i].DateVac}')", conn);
                        conn.Open();
                        command.ExecuteNonQuery();
                        conn.Close();
                    }                    
                }        
        }

        /// <summary>Прочитать из БД.</summary>
        /// <param name="param">Параметр.</param>
        /// <param name="searchText">Строка поиска.</param>
        /// <returns>Коллекция вакансий.</returns>
        public ObservableCollection<VacModel> ReadData(short param, string searchText)
        {
            var vacancies = new ObservableCollection<VacModel>();
            VacModel vac;
            var descrText = string.Empty;
            var query = string.Empty;

            switch (param)
            {
                case 1:
                    query = "SELECT * FROM vacancyInfo LIMIT 1000";
                    break;
                case 2:
                    query = $"SELECT * FROM vacancyInfo WHERE Name Like '%{searchText}%'";
                    break;
            }

                using (var conn = new SQLiteConnection(this.Connstring()))
                {
                    conn.Open();
                    var command = new SQLiteCommand(query, conn);

                    var reader = command.ExecuteReader();

                    foreach (DbDataRecord record in reader)
                    {
                        vac = new VacModel();
                        vac.Id = record["VacId"].ToString();
                        vac.Name = record["Name"].ToString();
                        vac.Company = record["Company"].ToString();
                        vac.Website = record["Website"].ToString();
                        vac.Salary = record["Salary"].ToString();
                        vac.Exp = record["Exp"].ToString();
                        vac.City = record["City"].ToString();
                        descrText = record["Description"].ToString();

                        vac.Description = new List<string>();

                        // преобразование описания вакансии в абзацы
                        var tempText = string.Empty;
                        int indx = 0;

                        while (descrText.Length > 0)
                        {
                            if (descrText.Length > 2)
                            {
                                indx = descrText.IndexOf("  ", 3);
                            }
                            else
                            {
                                break;
                            }

                            if (indx != -1)
                            {
                                tempText = descrText.Remove(indx);
                            }
                            else
                            {
                                vac.Description.Add(descrText);
                                break;
                            }

                            vac.Description.Add(tempText);
                            
                            if (tempText.Length == 0)
                            {
                                break;
                            }
                            else
                            {
                                descrText = descrText.Replace(tempText + "  ", string.Empty);
                            }
                        }
                        
                        vac.Address = record["Address"].ToString();
                        vac.Type = record["Type"].ToString();
                        vac.DateVac = record["DateVac"].ToString();
                        vac.Pic = AppDomain.CurrentDomain.BaseDirectory + "//img//" + vac.Id + ".jpg";
                        vacancies.Add(vac);
                    }

                    conn.Close();
                }

            return vacancies;
        }

        /// <summary>Сравнение считанных данных с сайта с данными в БД по id чтобы исключить дублирования данных в БД.</summary>
        /// <param name="siteVacansies">Вакансии.</param>
        /// <returns>Состояние.</returns>
        public bool CheckVacanciesInDB(List<VacModel> siteVacansies)
        {
            var vacId = new List<string>();
            var siteVacId = new List<string>();
            var newVacId = new List<string>();
            var siteVacTemp = new List<VacModel>();
            var flag = false;

            // вычитываем id всех вакансий из БД
                using (var conn = new SQLiteConnection(this.Connstring()))
                {
                    conn.Open();
                    var command = new SQLiteCommand("SELECT VacId FROM vacancyInfo", conn);

                    var reader = command.ExecuteReader();
                    foreach (DbDataRecord record in reader)
                    {
                        vacId.Add(record["VacId"].ToString());
                    }

                    conn.Close();
                }            

            // выбираем новые вакансии
            foreach (var v in siteVacansies)
            {
                siteVacId.Add(v.Id);                
            }

            var t1 = siteVacId.Except(vacId);

            foreach (var str in t1)
            {
                newVacId.Add(str);
            }

            foreach (var v in siteVacansies)
            {
                foreach (var s in newVacId)
                {
                    if (s == v.Id)
                    {
                        siteVacTemp.Add(v);
                    }
                }
            }

            // записываем новые вакансии в БД
                using (var conn = new SQLiteConnection(this.Connstring()))
                {
                    for (int i = 0; i < newVacId.Count(); i++)
                    {
                        var strDescr = string.Join("  ", siteVacTemp[i].Description.ToArray());

                        var command = new SQLiteCommand($"INSERT INTO 'vacancyInfo' ('VacId', 'Name', 'Company', 'Website', 'Salary', 'Exp', 'City', 'Description', 'Address', 'Type', 'DateVac') VALUES ('{siteVacTemp[i].Id}', '{siteVacTemp[i].Name}', '{siteVacTemp[i].Company}', '{siteVacTemp[i].Website}', '{siteVacTemp[i].Salary}', '{siteVacTemp[i].Exp}', '{siteVacTemp[i].City}', '{strDescr}', '{siteVacTemp[i].Address}', '{siteVacTemp[i].Type}', '{siteVacTemp[i].DateVac}')", conn);
                        conn.Open();
                        command.ExecuteNonQuery();
                        conn.Close();

                        flag = true;
                    }
                }            

            return flag;
        }

        private string Connstring()
        {
            string connStr = string.Format("Data Source={0};Version=3;Password={1};", DatabaseName, Pass);
            return connStr;
        }
    }
}