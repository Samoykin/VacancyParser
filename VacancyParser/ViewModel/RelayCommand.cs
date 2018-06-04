namespace VacancyParser.ViewModel
{
    using System;
    using System.Windows.Input;

    /// <summary>Команда.</summary>
    /// <typeparam name="T">Тип.</typeparam>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> execute;
        private readonly Predicate<T> canExecute;

        /// <summary>Initializes a new instance of the <see cref="RelayCommand{T}" /> class.</summary>
        /// <param name="execute">Логика выполнения.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="RelayCommand{T}" /> class.</summary>
        /// <param name="execute">Логика выполнения.</param>
        /// <param name="canExecute">Логика состояния выполнения.</param>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>Создано при вызове RaiseCanExecuteChanged.</summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>Определяет, можно ли выполнить эту команду <see cref="RelayCommand"/> в текущем состоянии.</summary>
        /// <param name="parameter">Данные, используемые командой. Если команда не требует передачи данных, этот объект можно установить равным NULL.</param>
        /// <returns>true, если команда может быть выполнена; в противном случае - false.</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null ? true : this.canExecute((T)parameter);
        }

        /// <summary>Выполняет <see cref="RelayCommand"/> текущей цели команды.</summary>
        /// <param name="parameter">Данные, используемые командой. Если команда не требует передачи данных, этот объект можно установить равным NULL.</param>
        public void Execute(object parameter)
        {
            this.execute((T)parameter);
        }

        /// <summary>Метод, используемый для создания события <see cref="CanExecuteChanged"/>чтобы показать, что возвращаемое значение <see cref="CanExecute"/> метод изменился.</summary>
        public void RaiseCanExecuteChanged()
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}