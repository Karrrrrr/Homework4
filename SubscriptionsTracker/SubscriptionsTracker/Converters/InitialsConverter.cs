using System;
using Microsoft.Maui.Controls;

namespace SubscriptionsTracker.Converters
{
    public class InitialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s))
            {
                var parts = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    var text = parts[0];
                    return text.Length >= 2 ? text.Substring(0, 2).ToUpperInvariant() : text.ToUpperInvariant();
                }
                return (parts[0][0].ToString() + parts[1][0].ToString()).ToUpperInvariant();
            }
            return "?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
