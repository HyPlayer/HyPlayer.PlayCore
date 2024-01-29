using Microsoft.UI.Xaml.Data;
using System;

namespace HyPlayer.PlayCore.Demo.AudioGraph.WinUI3.Converters
{
    public class TimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TimeSpan time)
            {
                return time.TotalMilliseconds;
            }
            else return 0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is double time)
            {
                return TimeSpan.FromMilliseconds(time);
            }
            else return TimeSpan.Zero;
        }
    }
}
