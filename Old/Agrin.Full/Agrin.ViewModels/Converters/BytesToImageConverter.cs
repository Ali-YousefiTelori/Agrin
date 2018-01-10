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
    public class BytesToImageConverter : IMultiValueConverter
    {
        //static Dictionary<object, BitmapImage> images = new Dictionary<object, BitmapImage>();
        public static Dictionary<System.Windows.Threading.Dispatcher, Dictionary<object, BitmapImage>> dispatcherImages = new Dictionary<System.Windows.Threading.Dispatcher, Dictionary<object, BitmapImage>>();

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var currentDispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

                if (values == null)
                    return null;

                if (values[0] == null)
                    return null;
                foreach (var item in values)
                {
                    if (item == System.Windows.DependencyProperty.UnsetValue || item == null)
                        return null;
                }


                if (!dispatcherImages.ContainsKey(currentDispatcher))
                    dispatcherImages.Add(currentDispatcher, new Dictionary<object, BitmapImage>());
                var images = dispatcherImages[currentDispatcher];
                if (values.Length == 2)
                {
                    if (values[1] != null && images.ContainsKey(values[1]))
                    {
                        return images[values[1]];
                    }
                    var bytes = values[0] as byte[];
                    using (MemoryStream byteStream = new MemoryStream(bytes))
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = byteStream;
                        bi.EndInit();
                        bi.Freeze();
                        images.Add(values[1], bi);
                        return bi;
                    }
                }
                else
                {
                    var bytes = values[0] as byte[];
                    using (MemoryStream byteStream = new MemoryStream(bytes))
                    {
                        BitmapImage bi = new BitmapImage();
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = byteStream;
                        bi.EndInit();
                        bi.Freeze();
                        return bi;
                    }
                }
            }
            catch (Exception e)
            {
                Agrin.Log.AutoLogger.LogError(e, "BytesToImageConverter");
                return null;
            }
        }
        public object[] ConvertBack(object value, Type[] targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
