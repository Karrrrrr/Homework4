using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SubscriptionsTracker.Models;

namespace SubscriptionsTracker.Services
{
    public class SubscriptionService
    {
        private readonly ISubscriptionRepository _repo;

        public SubscriptionService(ISubscriptionRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Subscription>> GetAllAsync() => _repo.GetAllAsync();

        public Task AddAsync(Subscription s) => _repo.AddAsync(s);

        public Task UpdateAsync(Subscription s) => _repo.UpdateAsync(s);

        public Task DeleteAsync(Guid id) => _repo.DeleteAsync(id);

        public async Task<decimal> GetMonthlyTotalAsync(int year, int month)
        {
            var all = await _repo.GetAllAsync();
            // estimate monthly spend by summing amounts for subscriptions with monthly frequency
            var total = all.Where(s => s.IsActive)
                           .Sum(s => s.Amount * FrequencyMultiplierForMonth(s.Frequency, year, month));
            return total;
        }

        public async Task<decimal> GetYearTotalAsync(int year)
        {
            var all = await _repo.GetAllAsync();
            var total = all.Where(s => s.IsActive)
                           .Sum(s => s.Amount * FrequencyMultiplierForYear(s.Frequency));
            return total;
        }

        public async Task<decimal> GetAllTimeTotalAsync()
        {
            var all = await _repo.GetAllAsync();
            // approximate: yearly*years since start + monthly*(months since start)
            decimal sum = 0m;
            var now = DateTime.UtcNow;
            foreach (var s in all.Where(s => s.IsActive))
            {
                var months = Math.Max(1, (now.Year - s.StartDate.Year) * 12 + now.Month - s.StartDate.Month);
                sum += s.Amount * FrequencyMultiplierForMonth(s.Frequency, now.Year, now.Month) * months;
            }
            return sum;
        }

        public async Task<decimal[]> GetLast12MonthsBreakdownAsync()
        {
            var all = (await _repo.GetAllAsync()).Where(s => s.IsActive).ToList();
            var now = DateTime.UtcNow;
            var result = new decimal[12];
            for (int i = 0; i < 12; i++)
            {
                var dt = now.AddMonths(-11 + i);
                var monthTotal = all.Sum(s => s.Amount * FrequencyMultiplierForMonth(s.Frequency, dt.Year, dt.Month));
                result[i] = monthTotal;
            }
            return result;
        }

        public async Task<Dictionary<Category, decimal>> GetCategoryTotalsForMonthAsync(int year, int month)
        {
            var all = (await _repo.GetAllAsync()).Where(s => s.IsActive);
            var dict = new Dictionary<Category, decimal>();
            foreach (Category c in Enum.GetValues(typeof(Category))) dict[c] = 0m;
            foreach (var s in all)
            {
                var amt = s.Amount * FrequencyMultiplierForMonth(s.Frequency, year, month);
                dict[s.Category] += amt;
            }
            return dict;
        }

        public async Task<IEnumerable<(string Name, decimal Amount)>> GetTopSubscriptionsYearAsync(int year, int topN = 5)
        {
            var all = (await _repo.GetAllAsync()).Where(s => s.IsActive);
            var list = all.Select(s => (s.Name, Amount: s.Amount * FrequencyMultiplierForYear(s.Frequency))).OrderByDescending(x => x.Amount).Take(topN);
            return list;
        }

        private decimal FrequencyMultiplierForMonth(Frequency freq, int year, int month)
        {
            // Simplified: if yearly then divide by 12, monthly => 1, weekly => 52/12, etc.
            return freq switch
            {
                Frequency.Daily => 30m,
                Frequency.Weekly => 52m / 12m,
                Frequency.Monthly => 1m,
                Frequency.Quarterly => 1m / 3m,
                Frequency.Yearly => 1m / 12m,
                Frequency.Custom => 1m,
                _ => 1m,
            };
        }

        private decimal FrequencyMultiplierForYear(Frequency freq)
        {
            return freq switch
            {
                Frequency.Daily => 365m,
                Frequency.Weekly => 52m,
                Frequency.Monthly => 12m,
                Frequency.Quarterly => 4m,
                Frequency.Yearly => 1m,
                Frequency.Custom => 12m,
                _ => 12m,
            };
        }
    }
}
