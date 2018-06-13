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
    public class DbConnect
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

                var command = new SQLiteCommand(query, conn);
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
                    for (var i = 0; i < vacancies.Count(); i++)
                    {
                        var strDescr = string.Join(" ", vacancies[i].Description.ToArray());

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
                        var vac = new VacModel
                        {
                            Id = record["VacId"].ToString(),
                            Name = record["Name"].ToString(),
                            Company = record["Company"].ToString(),
                            Website = record["Website"].ToString(),
                            Salary = record["Salary"].ToString(),
                            Exp = record["Exp"].ToString(),
                            City = record["City"].ToString()
                        };

                        var descrText = record["Description"].ToString();

                        vac.Description = new List<string>();

                        // Преобразование описания вакансии в абзацы
                        while (descrText.Length > 0)
                        {
                            var indx = 0;
                            if (descrText.Length > 2)
                            {
                                indx = descrText.IndexOf("  ", 3, StringComparison.Ordinal);
                            }
                            else
                            {
                                break;
                            }

                            string tempText;
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
                        vac.Pic = $"{AppDomain.CurrentDomain.BaseDirectory}/img/{vac.Id}.jpg";
                        vacancies.Add(vac);
                    }

                    conn.Close();
                }

            return vacancies;
        }

        /// <summary>Сравнение считанных данных с сайта с данными в БД по id чтобы исключить дублирования данных в БД.</summary>
        /// <param name="siteVacansies">Вакансии.</param>
        /// <returns>Состояние.</returns>
        public bool CheckVacanciesInDb(List<VacModel> siteVacansies)
        {
            var vacId = new List<string>();
            var siteVacTemp = new List<VacModel>();
            var flag = false;

            // вычитываем id всех вакансий из БД
                using (var conn = new SQLiteConnection(this.Connstring()))
                {
                    conn.Open();
                    var command = new SQLiteCommand("SELECT VacId FROM vacancyInfo", conn);

                    var reader = command.ExecuteReader();
                    vacId.AddRange(from DbDataRecord record in reader select record["VacId"].ToString());

                    conn.Close();
                }            

            // выбираем новые вакансии
            var siteVacId = siteVacansies.Select(v => v.Id).ToList();

            var t1 = siteVacId.Except(vacId);

            var newVacId = t1.ToList();

            foreach (var v in siteVacansies)
            {
                siteVacTemp.AddRange(from s in newVacId where s == v.Id select v);
            }

            // записываем новые вакансии в БД
                using (var conn = new SQLiteConnection(this.Connstring()))
                {
                    for (var i = 0; i < newVacId.Count(); i++)
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
            var connStr = $"Data Source={DatabaseName};Version=3;Password={Pass};";
            return connStr;
        }
    }
}