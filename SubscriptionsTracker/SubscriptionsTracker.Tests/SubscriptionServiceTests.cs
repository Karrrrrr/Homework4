using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SubscriptionsTracker.Models;
using SubscriptionsTracker.Services;

namespace SubscriptionsTracker.Tests
{
    class TestSubscriptionRepository : ISubscriptionRepository
    {
        private readonly Dictionary<Guid, Subscription> _store = new();
        public Task AddAsync(Subscription subscription)
        {
            _store[subscription.Id] = subscription;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            _store.Remove(id);
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

        public Task<IEnumerable<Subscription>> GetDueTomorrowAsync()
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);
            var list = _store.Values.Where(s => s.NextBillingDate.Date == tomorrow).AsEnumerable();
            return Task.FromResult(list);
        }

        public Task UpdateAsync(Subscription subscription)
        {
            _store[subscription.Id] = subscription;
            return Task.CompletedTask;
        }
    }

    [TestFixture]
    public class SubscriptionServiceTests
    {
        [Test]
        public async Task GetMonthlyTotalAsync_CalculatesExpectedSum()
        {
            var repo = new TestSubscriptionRepository();
            var svc = new SubscriptionService(repo);

            var s1 = new Subscription { Name = "MonthlyA", Amount = 10m, Frequency = Frequency.Monthly, IsActive = true, StartDate = DateTime.UtcNow };
            var s2 = new Subscription { Name = "YearlyB", Amount = 120m, Frequency = Frequency.Yearly, IsActive = true, StartDate = DateTime.UtcNow };

            await svc.AddAsync(s1);
            await svc.AddAsync(s2);

            var total = await svc.GetMonthlyTotalAsync(DateTime.UtcNow.Year, DateTime.UtcNow.Month);

            // Monthly: s1 contributes 10, s2 contributes 120/12 = 10 => total 20
            Assert.AreEqual(20m, Math.Round(total, 2));
        }

        [Test]
        public async Task GetCategoryTotalsForMonthAsync_AggregatesPerCategory()
        {
            var repo = new TestSubscriptionRepository();
            var svc = new SubscriptionService(repo);

            var s1 = new Subscription { Name = "VideoSub", Amount = 30m, Frequency = Frequency.Monthly, Category = Category.Video, IsActive = true, StartDate = DateTime.UtcNow };
            var s2 = new Subscription { Name = "CloudSub", Amount = 120m, Frequency = Frequency.Yearly, Category = Category.Cloud, IsActive = true, StartDate = DateTime.UtcNow };

            await svc.AddAsync(s1);
            await svc.AddAsync(s2);

            var dict = await svc.GetCategoryTotalsForMonthAsync(DateTime.UtcNow.Year, DateTime.UtcNow.Month);

            Assert.AreEqual(30m, dict[Category.Video]);
            // Cloud: 120/12 = 10
            Assert.AreEqual(10m, Math.Round(dict[Category.Cloud], 2));
        }

        [Test]
        public async Task GetTopSubscriptionsYearAsync_ReturnsTopByYearlyAmount()
        {
            var repo = new TestSubscriptionRepository();
            var svc = new SubscriptionService(repo);

            var s1 = new Subscription { Name = "SmallMonthly", Amount = 10m, Frequency = Frequency.Monthly, IsActive = true, StartDate = DateTime.UtcNow };
            var s2 = new Subscription { Name = "BigYearly", Amount = 300m, Frequency = Frequency.Yearly, IsActive = true, StartDate = DateTime.UtcNow };

            await svc.AddAsync(s1);
            await svc.AddAsync(s2);

            var top = (await svc.GetTopSubscriptionsYearAsync(DateTime.UtcNow.Year, topN: 2)).ToList();

            // s1 yearly = 10*12 = 120, s2 yearly = 300
            Assert.AreEqual(2, top.Count);
            Assert.AreEqual("BigYearly", top[0].Name);
            Assert.AreEqual(300m, Math.Round(top[0].Amount, 2));
        }
    }
}
