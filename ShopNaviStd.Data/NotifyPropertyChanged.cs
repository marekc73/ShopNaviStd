using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopNavi.Data
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        protected event PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this.propertyChanged += value;
            }
            remove
            {
                this.propertyChanged -= value;
            }
        }

        public virtual void OnPropertyChanged(string propertyName)
        {
            if (this.propertyChanged != null)
            {
                this.propertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
