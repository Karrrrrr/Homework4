using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SubscriptionsTracker.Services;
using SubscriptionsTracker.ViewModels;
using SubscriptionsTracker.Models;
using SubscriptionsTracker.Notifications;
using SubscriptionsTracker.Views;
using SubscriptionsTracker.Data;
using System.IO;
using System;

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
            // Configure SQLite database path
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "subscriptions.db");
            builder.Services.AddDbContext<SubscriptionsDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            builder.Services.AddSingleton<ISubscriptionRepository, EfSubscriptionRepository>();
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

            var app = builder.Build();

            // Ensure database is created
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SubscriptionsDbContext>();
                db.Database.EnsureCreated();
            }

            return app;
        }
    }
}

