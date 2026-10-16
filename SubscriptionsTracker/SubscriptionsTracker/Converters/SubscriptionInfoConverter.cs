using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using SubscriptionsTracker.Models;

namespace SubscriptionsTracker.Converters
{
    public class SubscriptionInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Subscription s)
            {
                var cat = s.Category.ToString();
                // map english enum names to russian labels
                var catRu = s.Category switch
                {
                    Category.Video => "Видео",
                    Category.Music => "Музыка",
                    Category.Cloud => "Облако",
                    Category.Work => "Работа",
                    _ => "Другое",
                };
                // Показываем дату начала подписки, а не текущую или дату следующего платежа
                var date = s.StartDate.ToString("d MMM", new CultureInfo("ru-RU"));
                return $"{catRu} • {date}";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
