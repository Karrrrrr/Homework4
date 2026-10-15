using System;
using System.Threading.Tasks;
using SubscriptionsTracker.Services;

namespace SubscriptionsTracker.Notifications
{
    // Placeholder scheduler. Platform-specific implementation required for real push/local notifications.
    public class NotificationScheduler : INotificationScheduler
    {
        private readonly ISubscriptionRepository _repo;

        public NotificationScheduler(ISubscriptionRepository repo)
        {
            _repo = repo;
        }

        public Task ScheduleDailyChecksAsync()
        {
            // In a real app this would integrate with platform notification APIs
            // Here we keep a simple placeholder that could be called from startup to schedule background work.
            return Task.CompletedTask;
        }
    }
}
