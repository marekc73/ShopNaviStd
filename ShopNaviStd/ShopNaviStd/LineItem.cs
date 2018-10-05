using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShopNavi
{
    public class LineItem
    {
        private CreateListVM parent;
        public LineItem(string txt, CreateListVM parentArg)
        {
            this.Text = txt;
            this.parent = parentArg;
            this.DeleteCmd = new Command((item) =>
            {
                this.Parent.Lines.Remove(Me);
            });

        }

        public string Text { get; set; }

        public LineItem Me
        {
            get
            {
                return this;
            }
        }

        public CreateListVM Parent
        {
            get
            {
                return this.parent;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }

        public ICommand DeleteCmd { get; protected set; }
    }
}
