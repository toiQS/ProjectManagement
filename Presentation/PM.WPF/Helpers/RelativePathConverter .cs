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
    public class RelativePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var getFullPath = Assembly.GetExecutingAssembly().Location;
            var getProjectSpecificPath = getFullPath.Split("\\bin")[0];
            if (value is string path)
            {
                // Chỉnh sửa để sử dụng đường dẫn tuyệt đối trong pack URI cho WPF
                var uri = new Uri($"{getProjectSpecificPath}{path}", UriKind.Absolute);
                return new BitmapImage(uri);
            }
            return null;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
