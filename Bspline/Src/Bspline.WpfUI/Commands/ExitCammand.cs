using System;
using System.Windows;
using System.Windows.Input;

namespace Bspline.WpfUI.Commands
{
    internal class ExitCammand:RelayCommand
    {
        private Action ExitAction { get; set; }

        public ExitCammand(Action exitAction)
        {
            ExitAction = exitAction;
        }
        
        public new bool CanExecute(object parameter,IInputElement a)
        {
            return this.ExitAction != null;
        }

        public void Execute( object parameter )
        {
            this.Execute(parameter,null);
        }

        public new void Execute(object parameter, IInputElement a)
        {
            this.ExitAction();
            this.ExitAction = null;
        }
    }

    public class RelayCommand : ICommand 
{ 
#region Fields 
readonly Action<object> _execute; readonly Predicate<object> _canExecute; #endregion // Fields #region Constructors public RelayCommand(Action<object> execute) : this(execute, null) { } public RelayCommand(Action<object> execute, Predicate<object> canExecute) { if (execute == null) throw new ArgumentNullException("execute"); _execute = execute; _canExecute = canExecute; } #endregion // Constructors #region ICommand Members [DebuggerStepThrough] public bool CanExecute(object parameter) { return _canExecute == null ? true : _canExecute(parameter); } public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } } public void Execute(object parameter) { _execute(parameter); } #endregion // ICommand Members }
}
