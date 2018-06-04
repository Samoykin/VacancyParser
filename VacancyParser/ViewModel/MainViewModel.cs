﻿namespace VacancyParser.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Helpers;
    using Model;

    /// <summary>Главная ViewModel.</summary>
    public class MainViewModel
    {
        private string dataBaseName = "VacancyDB.sqlite";
        private DBConnect dbc = new DBConnect();
        private bool flag = false;
        private DispatcherTimer checkDownloadData = new DispatcherTimer();
        private string logText = string.Empty;
        private LogFile logFile = new LogFile();

        /// <summary>Initializes a new instance of the <see cref="MainViewModel" /> class.</summary>
        public MainViewModel()
        {
            if (!File.Exists(this.dataBaseName))
            {
                this.dbc.CreateBase();
                this.dbc.CreateTable();
            }

            // Сортировка
            this.SortNameAscCommand = new Command(arg => this.ClickMethodSortNameAsc());
            this.SortNameDescCommand = new Command(arg => this.ClickMethodSortNameDesc());

            this.ClickSearch = new Command(arg => this.ClickMethodSearch());

            this.Vacancies = new ObservableCollection<VacModel> { };
            this.VacanciesT = new ObservableCollection<VacModel> { };
            this.Misc = new Misc { };

            this.Vacancies = this.dbc.ReadData(1, string.Empty);

            this.Misc.VacCount = this.Vacancies.Count();

            this.Misc.SelectedIndex = 0;
            this.Misc.SearchTypeSite = true;

            this.logText = DateTime.Now.ToString() + "|event|MainViewModel - MainViewModel| Запуск приложения VacancyParser";
            this.logFile.WriteLog(this.logText);
        }

        /// <summary>Команда ListSelChang.</summary>
        public ICommand ListSelChangCommand { get; set; }

        /// <summary>Команда сортировки по имени по возрастанию.</summary>
        public ICommand SortNameAscCommand { get; set; }

        /// <summary>Команда сортировки по имени по убыванию.</summary>
        public ICommand SortNameDescCommand { get; set; }

        /// <summary>Команда поиск.</summary>
        public ICommand ClickSearch { get; set; }

        /// <summary>Коллекция акансий.</summary>
        public ObservableCollection<VacModel> Vacancies { get; set; }

        /// <summary>Коллекция акансийT.</summary>
        public ObservableCollection<VacModel> VacanciesT { get; set; }

        /// <summary>Параметры.</summary>
        public Misc Misc { get; set; }

        // Загрузка данных с сайта асинхронно и запись в БД
        private async void DataDownload(string searchText)
        {
            HtmlParser htmlParser = new HtmlParser();
            List<VacModel> siteVacansies;

            siteVacansies = await Task<List<VacModel>>.Factory.StartNew(() =>
            {
                return htmlParser.Parse(searchText);
            });

            this.flag = this.dbc.CheckVacanciesInDB(siteVacansies);
        }
        
        // Сортировка по возрастанию
        private void ClickMethodSortNameAsc()
        {
            ObservableCollection<VacModel> vacanciesTemp;

            vacanciesTemp = this.Vacancies;
            vacanciesTemp = new ObservableCollection<VacModel>(vacanciesTemp.OrderBy(a => a.Name));
            this.Vacancies.Clear();
            foreach (VacModel ee in vacanciesTemp)
            {
                this.Vacancies.Add(ee);
            }

            this.Misc.SelectedIndex = 0;
        }

        // Сортировка по убыванию
        private void ClickMethodSortNameDesc()
        {
            ObservableCollection<VacModel> vacanciesTemp;

            vacanciesTemp = this.Vacancies;
            vacanciesTemp = new ObservableCollection<VacModel>(vacanciesTemp.OrderByDescending(a => a.Name));
            this.Vacancies.Clear();
            foreach (VacModel ee in vacanciesTemp)
            {
                this.Vacancies.Add(ee);
            }

            this.Misc.SelectedIndex = 0;
        }

        // Поиск
        private void ClickMethodSearch()
        {
            if (this.Misc.SearchTypeSite == true)
            {
                this.Misc.StatusUpd = "Вычитывание данных с сайта";
                this.flag = false;

                string searchPrepare = string.Empty;
                if (this.Misc.SearchText.IndexOf(" ") != -1)
                {
                    searchPrepare = this.Misc.SearchText.Replace(" ", "+");
                }
                else
                {
                    searchPrepare = this.Misc.SearchText;
                }

                this.DataDownload(searchPrepare);

                this.checkDownloadData.Interval = new TimeSpan(0, 0, 1);
                this.checkDownloadData.Tick += new EventHandler(this.CheckFlag);
                this.checkDownloadData.Start();

                this.logText = DateTime.Now.ToString() + "|event|MainViewModel - ClickMethodSearch| Поиск по сайту по слову/фразе " + searchPrepare;
                this.logFile.WriteLog(this.logText);
            }
            else
            {
                this.DBSearch(this.Misc.SearchText);

                this.logText = DateTime.Now.ToString() + "|event|MainViewModel - ClickMethodSearch| Поиск по БД по слову/фразе " + Misc.SearchText;
                this.logFile.WriteLog(this.logText);
            }             
        }

        // Поиск в БД
        private void DBSearch(string searchText)
        {
            this.VacanciesT.Clear();
            this.VacanciesT = this.dbc.ReadData(2, searchText);

            this.Vacancies.Clear();
            foreach (VacModel vm in this.VacanciesT)
            {
                this.Vacancies.Add(vm);
            }

            this.Misc.SelectedIndex = 0;

            this.Misc.VacCount = this.Vacancies.Count();
        }

        // Проверка загрузки данных чтобы поменять статус
        private void CheckFlag(object sender, EventArgs e)
        {
            if (this.flag == true)
            {
                this.Misc.StatusUpd = "Данные обновлены";
                this.DBSearch(this.Misc.SearchText);
                this.checkDownloadData.Stop();
            }
        }
    }
}