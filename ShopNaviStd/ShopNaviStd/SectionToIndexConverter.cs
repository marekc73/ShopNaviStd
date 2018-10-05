using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ShopNavi.Data;

namespace ShopNavi
{
    public class SectionToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Section it = value as Section;
            return it.Index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return  StoreFactory.CurrentStore.Sections[(Int32)value];
        }
    }
}
