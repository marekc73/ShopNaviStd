using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Xamarin.Forms;

namespace ShopNavi.Data
{
    public class Section : OrderLineVM
    {
        private string name = string.Empty;
        private int id = 0;

        [XmlAttribute]
        public string Name
        { 
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        [XmlAttribute]
        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
                this.OnPropertyChanged("Id");
            }
        }

        ObservableCollection<int> items = new ObservableCollection<int>();

        [XmlIgnore]
        public ObservableCollection<int> ItemLinks
        {
            get
            {
                return this.items;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override void Save()
        {
            //throw new NotImplementedException();
        }

        [XmlIgnore]
        public override string MoveLabel
        {
            get
            {
                try
                {
                    OrderContainerVM<Store, Section> element = this.Parent as OrderContainerVM<Store, Section>;
                    if (element != null)
                    {
                        return (element.Order.LineForMove != null && element.Order.LineForMove.LineMoveStatus == MoveStatus.Started) ? Section.Labels[1] : Section.Labels[0];
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                catch(Exception ex)
                {
                    StoreFactory.CurrentVM.Logs.Add(ex.Message);
                    StoreFactory.CurrentVM.Logs.Add(ex.StackTrace);
                    return string.Empty;
                }
            }
        }

    }
}
