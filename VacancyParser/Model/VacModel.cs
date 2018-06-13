namespace VacancyParser.Model
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>Вакансия.</summary>
    public class VacModel : INotifyPropertyChanged
    {
        #region Fields

        private string id;
        private string name;
        private string company;
        private string website;
        private string salary;
        private string exp;
        private string city;
        private List<string> description;
        private string address;
        private string type;
        private string dateVac;
        private string pic;

        #endregion

        /// <summary>Событие изменения свойства.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        /// <summary>Id.</summary>
        public string Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    this.OnPropertyChanged("Id");
                }
            }
        }

        /// <summary>Название.</summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>Компания.</summary>
        public string Company
        {
            get
            {
                return this.company;
            }

            set
            {
                if (this.company != value)
                {
                    this.company = value;
                    this.OnPropertyChanged("Company");
                }
            }
        }

        /// <summary>Сайт.</summary>
        public string Website
        {
            get
            {
                return this.website;
            }

            set
            {
                if (this.website != value)
                {
                    this.website = value;
                    this.OnPropertyChanged("Website");
                }
            }
        }

        /// <summary>Зарплата.</summary>
        public string Salary
        {
            get
            {
                return this.salary;
            }

            set
            {
                if (this.salary != value)
                {
                    this.salary = value;
                    this.OnPropertyChanged("Salary");
                }
            }
        }

        /// <summary>Опыт.</summary>
        public string Exp
        {
            get
            {
                return this.exp;
            }

            set
            {
                if (this.exp != value)
                {
                    this.exp = value;
                    this.OnPropertyChanged("Exp");
                }
            }
        }

        /// <summary>Город.</summary>
        public string City
        {
            get
            {
                return this.city;
            }

            set
            {
                if (this.city != value)
                {
                    this.city = value;
                    this.OnPropertyChanged("City");
                }
            }
        }

        /// <summary>Описание.</summary>
        public List<string> Description
        {
            get
            {
                return this.description;
            }

            set
            {
                if (this.description != value)
                {
                    this.description = value;
                    this.OnPropertyChanged("Description");
                }
            }
        }

        /// <summary>Адрес.</summary>
        public string Address
        {
            get
            {
                return this.address;
            }

            set
            {
                if (this.address != value)
                {
                    this.address = value;
                    this.OnPropertyChanged("Address");
                }
            }
        }

        /// <summary>Тип.</summary>
        public string Type
        {
            get
            {
                return this.type;
            }

            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.OnPropertyChanged("Type");
                }
            }
        }

        /// <summary>Дата вакансии.</summary>
        public string DateVac
        {
            get
            {
                return this.dateVac;
            }

            set
            {
                if (this.dateVac != value)
                {
                    this.dateVac = value;
                    this.OnPropertyChanged("DateVac");
                }
            }
        }

        /// <summary>Картинка.</summary>
        public string Pic
        {
            get
            {
                return this.pic;
            }

            set
            {
                if (this.pic != value)
                {
                    this.pic = value;
                    this.OnPropertyChanged("Pic");
                }
            }
        }

        #endregion

        #region Implement INotyfyPropertyChanged members

        /// <summary>Изменения свойства.</summary>
        /// <param name="propertyName">Имя свойства.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}