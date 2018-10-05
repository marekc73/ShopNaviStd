using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShopNavi.Data;
using Xamarin.Forms;

namespace ShopNavi
{
    public class ValidToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Section sect = value as Section;
            if (sect != null)
            {
                return sect.LineMoveStatus == MoveStatus.Started ? Color.Purple : Color.Default;
            }
            else if (sect != null)
            {
                return sect.LineMoveStatus == MoveStatus.Finished ? Color.Blue : Color.Default;
            }
            else
            {
                bool it = (bool)value;
                return it ? Color.Green : Color.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }
    }
}
