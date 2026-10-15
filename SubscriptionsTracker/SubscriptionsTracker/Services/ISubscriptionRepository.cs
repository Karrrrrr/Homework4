using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SubscriptionsTracker.Models;

namespace SubscriptionsTracker.Services
{
    public interface ISubscriptionRepository
    {
        Task<IEnumerable<Subscription>> GetAllAsync();
        Task<Subscription?> GetAsync(Guid id);
        Task AddAsync(Subscription subscription);
        Task UpdateAsync(Subscription subscription);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Subscription>> GetDueTomorrowAsync();
    }
}
