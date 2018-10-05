using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public class PageItem : NotifyPropertyChanged
    {
        private string title;
        private System.Type type;
        private string icon;
        private BaseVM vm;
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
                OnPropertyChanged("Title");
            }
        }

        public string Icon
        {
            get
            {
                return this.icon;
            }
            set
            {
                this.icon = value;
                OnPropertyChanged("Icon");
            }
        }
        public Type Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
                OnPropertyChanged("Type");
            }
        }
        public BaseVM VM
        {
            get
            {
                return this.vm;
            }
            set
            {
                this.vm = value;
                OnPropertyChanged("VM");
            }
        }

        public ContentPage Page
        {
            get;
            set;
        }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
