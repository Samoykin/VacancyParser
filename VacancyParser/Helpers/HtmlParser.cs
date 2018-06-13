namespace VacancyParser.Helpers
{    
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using HtmlAgilityPack;
    using Model;

    /// <summary>Парсер страницы сайта.</summary>
    public class HtmlParser
    {
        private List<VacModel> vacansies;
        private VacModel vac;

        /// <summary>Распарсить.</summary>
        /// <param name="searchText">Строка поиска.</param>
        /// <returns>Список вакансий.</returns>
        public List<VacModel> Parse(string searchText)
        {
            this.vacansies = new List<VacModel>();
            
            string fullHtml;
            var picWebPath = string.Empty;
            var webPath = string.Empty;          
   
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var address2 = @"https://hh.ru/search/vacancy?enable_snippets=true&area=113&text=" + searchText + @"&search_field=name&clusters=true&page=0";
                    fullHtml = client.DownloadString(address2);
                }

                // При помощи HtmlAgilityPack парсим сайт
                var doc = new HtmlDocument();
                doc.LoadHtml(fullHtml);

                // список вакансий
                var htmlVacList = doc.DocumentNode.SelectNodes("//a[@class='bloko-link HH-LinkModifier']");
                var htmlVacList1 = doc.DocumentNode.SelectNodes("//a[@class='search-result-item__name search-result-item__name_standard_plus HH-LinkModifier']");

                if (htmlVacList == null)
                {
                    htmlVacList = htmlVacList1;
                }
                else if (htmlVacList1 != null)
                {
                    foreach (var n in htmlVacList1)
                    {
                        htmlVacList.Add(n);
                    }
                }

                if (htmlVacList != null)
                {
                    // заходим на страничку каждой вакансии
                    foreach (var n in htmlVacList)
                    {
                        if (n.Attributes["href"] != null)
                        {
                            this.vac = new VacModel();
                            var vacLink = n.Attributes["href"].Value;
                            string vacDescription;
                            var descrText = string.Empty; 
                            var companyNameTemp = string.Empty;

                            using (var client = new WebClient())
                            {
                                client.Encoding = Encoding.UTF8;
                                client.Headers["User-Agent"] = "Mozilla/5.0";
                                vacDescription = client.DownloadString(vacLink);
                            }

                            // страница вакансии
                            var docVac = new HtmlDocument();
                            docVac.LoadHtml(vacDescription);

                            // id выкансии
                            var pattern = @"\b(\d+)";
                            var regex = new Regex(pattern);
                            var match = regex.Match(vacLink);
                            if (match.Success)
                            {
                                this.vac.Id = match.Value;
                            }

                            // название вакансии
                            var htmlField = docVac.DocumentNode.SelectNodes("//a[@class='bloko-link bloko-link_list HH-LinkModifier']");
                            if (htmlField != null)
                            {
                                this.vac.Name = htmlField.FirstOrDefault()?.InnerText;
                            }

                            // название компании
                            htmlField = docVac.DocumentNode.SelectNodes("//a[@class='vacancy-company-name']");
                            if (htmlField != null)
                            {
                                companyNameTemp = htmlField.FirstOrDefault()?.InnerText;
                            }

                            if (companyNameTemp != null && companyNameTemp.IndexOf(@"< !--noindex-- >)", StringComparison.Ordinal) != -1)
                            {
                                this.vac.Company = companyNameTemp.Replace(@"< !--noindex-- >)", string.Empty);
                            }
                            else
                            {
                                this.vac.Company = companyNameTemp;
                            }

                            // зарплата
                            htmlField = docVac.DocumentNode.SelectNodes("//p[@class='vacancy-salary']");
                            if (htmlField != null)
                            {
                                this.vac.Salary = htmlField.FirstOrDefault()?.InnerText;
                            }

                            // описание
                            htmlField = docVac.DocumentNode.SelectNodes("//div[@class='g-user-content']");
                            if (htmlField != null)
                            {
                                descrText = htmlField.FirstOrDefault()?.InnerText;
                            }

                            // преобразование описания вакансии в абзацы
                            this.vac.Description = new List<string>();

                            while (!string.IsNullOrEmpty(descrText))
                            {
                                var indx = 0;
                                if (descrText.Length > 1)
                                {
                                    indx = descrText.IndexOf("  ", 2, StringComparison.Ordinal);
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
                                    this.vac.Description.Add(descrText);
                                    break;
                                }

                                this.vac.Description.Add(tempText);

                                if (tempText.Length == 0)
                                {
                                    break;
                                }
                                else
                                {
                                    descrText = descrText.Replace(tempText + "  ", string.Empty);
                                }
                            }

                            // Город
                            htmlField = docVac.DocumentNode.SelectNodes("//td[@class='l-content-colum-2 b-v-info-content']");
                            if (htmlField != null)
                            {
                                this.vac.City = htmlField.FirstOrDefault().InnerText;
                            }

                            // Адрес
                            htmlField = docVac.DocumentNode.SelectNodes("//div[@class='vacancy-address-text HH-Maps-ShowAddress-Address']");
                            if (htmlField != null)
                            {
                                this.vac.Address = htmlField.FirstOrDefault().InnerText;
                            }

                            // Навыки
                            htmlField = docVac.DocumentNode.SelectNodes("//span[@class='Bloko-TagList-Text']");
                            if (htmlField != null)
                            {
                                this.vac.Exp = htmlField.FirstOrDefault().InnerText;
                            }

                            // Тип занятости
                            htmlField = docVac.DocumentNode.SelectNodes("//div[@class='l-content-paddings']");
                            if (htmlField != null)
                            {
                                this.vac.Type = htmlField.FirstOrDefault().InnerText;
                            }

                            // Дата вакансии
                            htmlField = docVac.DocumentNode.SelectNodes("//p[@class='vacancy-creation-time']");
                            if (htmlField != null)
                            {
                                this.vac.DateVac = htmlField.FirstOrDefault().InnerText;
                            }

                            // Страница компании
                            htmlField = docVac.DocumentNode.SelectNodes("//a[@class='vacancy-company-name']");
                            if (htmlField != null)
                            {
                                webPath = htmlField.FirstOrDefault().FirstChild.GetAttributeValue("href", string.Empty);
                            }

                            this.vac.Website = $@"https://hh.ru{webPath}";

                            // Картинка
                            htmlField = docVac.DocumentNode.SelectNodes("//a[@class='vacancy-company-logo']");
                            if (htmlField != null)
                            {
                                picWebPath = htmlField.FirstOrDefault().FirstChild.GetAttributeValue("src", string.Empty);
                            }

                            this.vac.Pic = $@"{AppDomain.CurrentDomain.BaseDirectory}/img/{this.vac.Id}.jpg";
                            this.SavePic($@"https://hh.ru{picWebPath}", this.vac.Pic);
                        }

                        this.vacansies.Add(this.vac);
                    }
                }

            return this.vacansies;
        }

        private void SavePic(string source, string path)
        {
            if (!File.Exists(path))
            {
                var wc = new WebClient
                {
                    Credentials = CredentialCache.DefaultCredentials
                };

                wc.DownloadFile(source, path);
            }
        }
    }
}