using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using DesktopTasks.Models;

namespace DesktopTasks.Converters
{
    public class PriorityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskPriority priority)
            {
                return priority switch
                {
                    TaskPriority.High => new SolidColorBrush(Color.FromRgb(220, 53, 69)),    // Red
                    TaskPriority.Normal => new SolidColorBrush(Color.FromRgb(0, 123, 255)),   // Blue
                    TaskPriority.Low => new SolidColorBrush(Color.FromRgb(40, 167, 69)),      // Green
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
