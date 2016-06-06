using System;
using System.Windows.Input;

namespace Bspline.WpfUI.Commands
{
    /// <summary>
    /// Concreate implementation of the Command DesignPattern used by WPF's <see cref="System.Windows.Input.ICommand"/>
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Fields 
        /// <summary>
        /// Field which holds the action to perform
        /// </summary>
        private readonly Action<object> _execute;

        /// <summary>
        /// Field which holds predicat condition which needed to be verified before applying the action
        /// </summary>
        private readonly Predicate<object> _canExecute;

        #endregion

        #region Constructors 

        /// <summary>
        /// Constructor which always perform an action
        /// </summary>
        /// <param name="execute"></param>
        public RelayCommand(Action<object> execute) 
            : this(execute, null)
        {
        }

        /// <summary>
        /// Constructor which verifies the predicat before applying the action
        /// </summary>
        /// <param name="execute"></param>
        /// <param name="canExecute"></param>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion 

        #region ICommand Members 

        /// <summary>
        /// <see cref="System.Windows.Input.ICommand"/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute(parameter);
        }

        /// <summary>
        /// <see cref="System.Windows.Input.ICommand"/>
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// <see cref="System.Windows.Input.ICommand"/>
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            this._execute(parameter);
        }

        #endregion
    }
}