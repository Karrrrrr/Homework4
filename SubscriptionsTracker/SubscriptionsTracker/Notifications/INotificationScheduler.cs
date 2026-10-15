using System.Threading.Tasks;

namespace SubscriptionsTracker.Notifications
{
    public interface INotificationScheduler
    {
        Task ScheduleDailyChecksAsync();
    }
}
