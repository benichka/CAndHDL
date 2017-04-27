using System;
using System.Windows.Input;

namespace CAndHDL
{
    /// <summary>
    /// Command class used for a button that sends no parameters to
    /// drives it. Instead, the class takes 2 parameters in its construction
    /// to determine if the command is executable or not.
    /// </summary>
    public class Command : ICommand
    {
        /// <summary>Action to execute in case the command is executable</summary>
        private Action MethodToExecute = null;

        /// <summary>Method to determine whether to command is executable or not</summary>
        private Func<bool> MethodToDetermineCanExecute = null;

        /// <summary>
        /// Parameterized constructor
        /// </summary>
        /// <param name="methodToExecute">Method to execute in case the command is executable</param>
        /// <param name="methodToDetermineCanExecute">Method to determine whether to command is executable or not</param>
        public Command(Action methodToExecute, Func<bool> methodToDetermineCanExecute)
        {
            this.MethodToExecute = methodToExecute;
            this.MethodToDetermineCanExecute = methodToDetermineCanExecute;
        }

        /// <summary>
        /// Determine whether the command is executable or not
        /// </summary>
        /// <param name="parameter">optional parameters</param>
        /// <returns>True if the method can be executed, false otherwise</returns>
        public bool CanExecute(object parameter)
        {
            if (MethodToDetermineCanExecute == null)
            {
                return true;
            }
            else
            {
                return this.MethodToDetermineCanExecute();
            }
        }

        /// <summary>
        /// Execute the command
        /// </summary>
        /// <param name="parameter">optional parameters</param>
        public void Execute(object parameter)
        {
            this.MethodToExecute();
        }

        /// <summary>Event handler</summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raise the event indicating that something changed and we need
        /// to check if the command is now executable or not
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
