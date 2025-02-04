using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PM.WPF.Helpers
{
    public class RelativePathConverter 
    {
        public object Convert(string path)
        {
            var getFullPath = Assembly.GetExecutingAssembly().Location;
            var getProjectSpecificPath = getFullPath.Split("\\bin")[0];
            if(!string.IsNullOrEmpty(path))
            {
                var uri = new Uri($"{getProjectSpecificPath}{path}", UriKind.Absolute);
                return new BitmapImage(uri);
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
