using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using SubscriptionsTracker.Models;

namespace SubscriptionsTracker.Converters
{
    public class FrequencySuffixConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Frequency f)
            {
                return f switch
                {
                    Frequency.Daily => "в день",
                    Frequency.Weekly => "в неделю",
                    Frequency.Monthly => "в месяц",
                    Frequency.Quarterly => "в квартал",
                    Frequency.Yearly => "в год",
                    Frequency.Custom => "",
                    _ => string.Empty,
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
