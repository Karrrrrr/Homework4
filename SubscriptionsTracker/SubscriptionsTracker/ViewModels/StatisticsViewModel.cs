using System.Threading.Tasks;
using SubscriptionsTracker.Services;
using System.Collections.Generic;
using System.Linq;
using SubscriptionsTracker.Models;


namespace SubscriptionsTracker.ViewModels
{
    public class StatisticsViewModel : BindableObject
    {
        private readonly SubscriptionService _service;

        public decimal MonthlyTotal { get; private set; }
        public decimal YearTotal { get; private set; }
        public decimal Last12Total { get; private set; }
        public decimal AllTimeTotal { get; private set; }

        public decimal[] MonthlyBreakdown { get; private set; } = new decimal[12];
        public List<CategoryStat> CategoryStats { get; private set; } = new();
        public List<TopSubscription> TopSubscriptions { get; private set; } = new();

        public StatisticsViewModel(SubscriptionService service)
        {
            _service = service;
        }

        public async Task LoadAsync(int year, int month)
        {
            MonthlyTotal = await _service.GetMonthlyTotalAsync(year, month);
            YearTotal = await _service.GetYearTotalAsync(year);
            MonthlyBreakdown = await _service.GetLast12MonthsBreakdownAsync();
            AllTimeTotal = await _service.GetAllTimeTotalAsync();

            var last12 = MonthlyBreakdown.Sum();
            Last12Total = last12;

            var catTotals = await _service.GetCategoryTotalsForMonthAsync(year, month);
            var total = catTotals.Values.Sum();
            CategoryStats = catTotals.Select(kv => new CategoryStat { Category = kv.Key, Amount = kv.Value, Percent = total > 0 ? (double)(kv.Value / total * 100.0m) : 0 }).ToList();

            var tops = await _service.GetTopSubscriptionsYearAsync(year, 5);
            TopSubscriptions = tops.Select(t => new TopSubscription { Name = t.Name, Amount = t.Amount }).ToList();

            OnPropertyChanged(string.Empty); // refresh all bindings
        }
    }
}
