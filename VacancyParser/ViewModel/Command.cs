namespace VacancyParser.ViewModel
{
    using System;
    using System.Windows.Input;

    /// <summary>Команда.</summary>
    public class Command : ICommand
    {
        #region Constructor

        /// <summary>Initializes a new instance of the <see cref="Command" /> class.</summary>
        /// <param name="action">Команда.</param>
        public Command(Action<object> action)
        {
            this.ExecuteDelegate = action;
        }

        #endregion

        /// <summary>Событие изменения свойства.</summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #region Properties

        /// <summary>Свойство возможно ли выполнить команду.</summary>
        public Predicate<object> CanExecuteDelegate { get; set; }

        /// <summary>Свойство выполнить команду.</summary>
        public Action<object> ExecuteDelegate { get; set; }

        #endregion        

        #region ICommand Members

        /// <summary>Определяет, можно ли выполнить эту команду <see cref="Command"/> в текущем состоянии.</summary>
        /// <param name="parameter">Данные, используемые командой. Если команда не требует передачи данных, этот объект можно установить равным NULL.</param>
        /// <returns>true, если команда может быть выполнена; в противном случае - false.</returns>
        public bool CanExecute(object parameter)
        {
            if (this.CanExecuteDelegate != null)
            {
                return this.CanExecuteDelegate(parameter);
            }

            return true;
        }

        /// <summary>Выполняет <see cref="Command"/> текущей цели команды.</summary>
        /// <param name="parameter">Данные, используемые командой. Если команда не требует передачи данных, этот объект можно установить равным NULL.</param>
        public void Execute(object parameter)
        {
            if (this.ExecuteDelegate != null)
            {
                this.ExecuteDelegate(parameter);
            }
        }

        #endregion
    }
}