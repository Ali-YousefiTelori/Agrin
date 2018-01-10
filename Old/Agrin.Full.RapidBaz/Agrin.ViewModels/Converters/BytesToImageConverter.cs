using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Agrin.ViewModels.Converters
{
    public class BytesToImageConverter : IValueConverter
    {
        public bool IsInverse { get; set; }
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            var bytes = value as byte[];
            if (bytes == null)
                return null;
            try
            {
                using (MemoryStream byteStream = new MemoryStream(bytes))
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = byteStream;
                    bi.EndInit();
                    return bi;
                }
            }
            catch(Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "BytesToImageConverter");
                return null;
            }

        }
        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
