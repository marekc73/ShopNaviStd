using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public class OutputFilterConverter : IValueConverter
    {
        public static ObservableCollection<OutputLine> deletedList = null;
        public static ObservableCollection<OutputLine> fullList = null;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<OutputLine> list = value as ObservableCollection<OutputLine>;
            if (list != null)
            {
                var ret = new ObservableCollection<OutputLine>();
                foreach(var it in list.Where(x => x.Valid || StoreFactory.CurrentVM.ShowDeleted))
                {
                    ret.Add(it);
                }
                
                if (StoreFactory.CurrentVM.ShowDeleted)
                {
                    OutputFilterConverter.fullList = null;
                    list = OutputFilterConverter.fullList;
                }
                else
                {
                    OutputFilterConverter.fullList = list;

                    list = ret;
                }

                return list;
            }
            else
            {
                return new ObservableCollection<OutputLine>(); 
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
