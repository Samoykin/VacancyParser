namespace VacancyParser.Model
{
    using System.ComponentModel;

    /// <summary>Настройки.</summary>
    public class Misc : INotifyPropertyChanged
    {
        #region Fields

        private int selectedIndex;
        private int vacCount;
        private bool searchTypeSite;
        private string searchText;
        private string statusUpd;

        #endregion

        /// <summary>Событие изменения свойства.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        /// <summary>Выбрать индекс.</summary>
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                if (this.selectedIndex != value)
                {
                    this.selectedIndex = value;
                    this.OnPropertyChanged("SelectedIndex");
                }
            }
        }

        /// <summary>Количество вакансий.</summary>
        public int VacCount
        {
            get
            {
                return this.vacCount;
            }

            set
            {
                if (this.vacCount != value)
                {
                    this.vacCount = value;
                    this.OnPropertyChanged("VacCount");
                }
            }
        }

        /// <summary>Поиск по типу.</summary>
        public bool SearchTypeSite
        {
            get
            {
                return this.searchTypeSite;
            }

            set
            {
                if (this.searchTypeSite != value)
                {
                    this.searchTypeSite = value;
                    this.OnPropertyChanged("SearchTypeSite");
                }
            }
        }

        /// <summary>Поиск по тексту.</summary>
        public string SearchText
        {
            get
            {
                return this.searchText;
            }

            set
            {
                if (this.searchText != value)
                {
                    this.searchText = value;
                    this.OnPropertyChanged("SearchText");
                }
            }
        }

        /// <summary>Состояние обновления.</summary>
        public string StatusUpd
        {
            get
            {
                return this.statusUpd;
            }

            set
            {
                if (this.statusUpd != value)
                {
                    this.statusUpd = value;
                    this.OnPropertyChanged("StatusUpd");
                }
            }
        }

        #endregion

        #region Implement INotyfyPropertyChanged members        

        /// <summary>Изменения свойства.</summary>
        /// <param name="propertyName">Имя свойства.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}