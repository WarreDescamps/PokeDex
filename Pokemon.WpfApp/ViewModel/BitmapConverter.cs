using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PokeDex.WpfApp.ViewModel
{
    internal class BitmapConverter : IValueConverter
    {
        public static BitmapImage Convert(Bitmap? value)
        {
            if (value == null)
                return null;
            using (var memory = new MemoryStream())
            {
                value.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value as Bitmap);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
