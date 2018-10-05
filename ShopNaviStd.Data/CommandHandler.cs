using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ShopNavi.Data
{
    public class CommandHandler : ICommand
    {
        private Action _action = null;
        private Action<object> _actionObj = null;
        private Action<string> _actionString = null;

        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute = true)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public CommandHandler(Action<object> action, bool canExecute = true)
        {
            _actionObj = action;
            _canExecute = canExecute;
        }

        public CommandHandler(Action<string> action, bool canExecute = true)
        {
            _actionString = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_action != null)
            {
                _action();
            }
            else if(_actionObj != null)
            {
                _actionObj(parameter);
            }
            else if (_actionString != null)
            {
                _actionString(parameter.ToString());
            }

        }
    }
}
