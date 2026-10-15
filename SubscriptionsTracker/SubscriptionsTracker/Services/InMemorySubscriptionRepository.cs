using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubscriptionsTracker.Models;

namespace SubscriptionsTracker.Services
{
    public class InMemorySubscriptionRepository : ISubscriptionRepository
    {
        private readonly ConcurrentDictionary<Guid, Subscription> _store = new();

        public InMemorySubscriptionRepository()
        {
            // Seed with a couple of example subscriptions
            var s1 = new Subscription
            {
                Name = "VideoPlus",
                Category = Category.Video,
                Amount = 399.99m,
                Frequency = Frequency.Monthly,
                StartDate = DateTime.UtcNow.AddMonths(-3),
                NextBillingDate = DateTime.UtcNow.AddDays(5)
            };
            var s2 = new Subscription
            {
                Name = "CloudStorage",
                Category = Category.Cloud,
                Amount = 199.00m,
                Frequency = Frequency.Yearly,
                StartDate = DateTime.UtcNow.AddYears(-1),
                NextBillingDate = DateTime.UtcNow.AddDays(30)
            };
            _store[s1.Id] = s1;
            _store[s2.Id] = s2;
        }

        public Task AddAsync(Subscription subscription)
        {
            _store[subscription.Id] = subscription;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _store.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return Task.FromResult(_store.Values.AsEnumerable());
        }

        public Task<Subscription?> GetAsync(Guid id)
        {
            _store.TryGetValue(id, out var s);
            return Task.FromResult(s);
        }

        public Task UpdateAsync(Subscription subscription)
        {
            _store[subscription.Id] = subscription;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Subscription>> GetDueTomorrowAsync()
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);
            var list = _store.Values.Where(s => s.NextBillingDate.Date == tomorrow).AsEnumerable();
            return Task.FromResult(list);
        }
    }
}
