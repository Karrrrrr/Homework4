using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SubscriptionsTracker.Services;
using SubscriptionsTracker.ViewModels;
using SubscriptionsTracker.Models;
using SubscriptionsTracker.Notifications;
using SubscriptionsTracker.Views;

namespace SubscriptionsTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Dependency injection registrations for app structure
            builder.Services.AddSingleton<ISubscriptionRepository, InMemorySubscriptionRepository>();
            builder.Services.AddSingleton<SubscriptionService>();
            builder.Services.AddSingleton<INotificationScheduler, NotificationScheduler>();

            builder.Services.AddSingleton<MainViewModel>();
            // Register pages and viewmodels
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddSingleton<StatisticsViewModel>();
            builder.Services.AddSingleton<StatisticsPage>();

            // Subscription edit viewmodel and page per-instance
            builder.Services.AddTransient<SubscriptionViewModel>();
            builder.Services.AddTransient<Views.SubscriptionEditPage>();

            return builder.Build();
        }
    }
}

