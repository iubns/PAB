using System;
using System.Windows.Input;

namespace PAB.ViewModel
{ 
    #region DelegateCommand Class
    public class CommandHandler : ICommand
    {
        private readonly Func<bool> canExecute;
        private readonly Action execute;

        public CommandHandler(Action execute) : this(execute, null)
        {
        }

        public CommandHandler(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object o)
        {
            if (canExecute == null)
            {
                return true;
            }
            return canExecute();
        }

        public void Execute(object o)
        {
            execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    #endregion
}