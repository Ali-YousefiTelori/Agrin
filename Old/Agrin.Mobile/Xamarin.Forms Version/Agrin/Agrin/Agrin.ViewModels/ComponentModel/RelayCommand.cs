using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Agrin.ViewModels.ComponentModel
{
    //public static class RelayCommand
    //{
    //    public static Func<Action, Func<bool>, ICommand> Create { get; set; }
    //    public static Func<Action, ICommand> CreateAction { get; set; }

    //    public static ICommand CreateObj<T>(Action<T> action, Func<T, bool> func)
    //    {
    //        return RelayCommand<T>.Create(action, func);
    //    }

    //    public static ICommand CreateActionObj<T>(Action<T> action)
    //    {
    //        return RelayCommand<T>.CreateAction(action);
    //    }
    //}

    //public static class RelayCommand<T>
    //{
    //    public static Func<Action<T>, Func<T, bool>, ICommand> Create { get; set; }
    //    public static Func<Action<T>, ICommand> CreateAction { get; set; }
    //}
    public class RelayCommand : ICommand
    {
        private Func<object, bool> _canExecute;
        private Action<object> _execute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute) : this(delegate (object o)
        {
            execute();
        })
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
        }

        public RelayCommand(Action<object> execute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            this._execute = execute;
        }

        public RelayCommand(Action execute, Func<bool> canExecute) : this(delegate (object o)
        {
            execute();
        }, delegate (object o)
        {
            return canExecute();
        })
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute) : this(execute)
        {
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }
            this._canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (this._canExecute != null)
            {
                return this._canExecute(parameter);
            }
            return true;
        }

        public void ChangeCanExecute()
        {
            EventHandler canExecuteChanged = this.CanExecuteChanged;
            if (canExecuteChanged != null)
            {
                canExecuteChanged(this, EventArgs.Empty);
            }
        }

        public void Execute(object parameter)
        {
            this._execute(parameter);
        }
    }

    public sealed class RelayCommand<T> : RelayCommand
    {
        public RelayCommand(Action<T> execute) : base(delegate (object o)
        {
            execute((T)o);
        })
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
        }

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute) : base(delegate (object o)
        {
            execute((T)o);
        }, delegate (object o)
        {
            return canExecute((T)o);
        })
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }
            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }
        }
    }

}
