using System;
using System.Windows.Input;

namespace CAndHDL
{
    public class Command : ICommand
    {
        /// <summary>Method to execute if the command is executable</summary>
        private Action MethodToExecute = null;

        /// <summary>Method to determine if the command is executable</summary>
        private Func<bool> MethodToDetermineCanExecute = null;

        /// <summary>
        /// Defaut constructor
        /// </summary>
        /// <param name="methodToExecute">Method to execute if the command is executable</param>
        /// <param name="methodToDetermineCanExecute">Method to determine if the command is executable</param>
        public Command(Action methodToExecute, Func<bool> methodToDetermineCanExecute)
        {
            this.MethodToExecute = methodToExecute;
            this.MethodToDetermineCanExecute = methodToDetermineCanExecute;
        }

        /// <summary>
        /// Determine if the action associated to the command is executable
        /// </summary>
        /// <param name="parameter">TODO</param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (MethodToDetermineCanExecute == null)
            {
                return true;
            }
            else
            {
                // TODO: adapt the CanExecute
                return this.MethodToDetermineCanExecute();
            }
        }

        /// <summary>
        /// Code to execute if the command is executable
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            this.MethodToExecute();
        }

        /// <summary>Command event handler</summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Notify the subscriber
        /// </summary>
        /// <param name="sender">TODO</param>
        /// <param name="e">TODO</param>
        void RaiseCanExecuteChanged(object sender, object e)
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
