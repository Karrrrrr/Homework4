using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SubscriptionsTracker.Models;
using SubscriptionsTracker.Services;

namespace SubscriptionsTracker.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly SubscriptionService _service;
        public ObservableCollection<Subscription> Subscriptions { get; } = new();
        public ObservableCollection<Subscription> FilteredSubscriptions { get; } = new();

        private Category? _selectedCategory = null;
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(); ApplyFilter(_selectedCategory); }
        }
        private decimal _monthlyTotal;
        public decimal MonthlyTotal
        {
            get => _monthlyTotal;
            private set { _monthlyTotal = value; OnPropertyChanged(); }
        }

        public MainViewModel(SubscriptionService service)
        {
            _service = service;
        }

        public async Task LoadAsync()
        {
            Subscriptions.Clear();
            var all = await _service.GetAllAsync();
            foreach (var s in all)
                Subscriptions.Add(s);

            MonthlyTotal = await _service.GetMonthlyTotalAsync(System.DateTime.UtcNow.Year, System.DateTime.UtcNow.Month);

            ApplyFilter(SelectedCategory);
        }

        public void ApplyFilter(Category? category)
        {
            FilteredSubscriptions.Clear();
            var items = Subscriptions.AsEnumerable();
            if (category != null)
                items = items.Where(s => s.Category == category.Value);
            foreach (var s in items)
                FilteredSubscriptions.Add(s);
        }

        public async Task AddAsync(Subscription s)
        {
            await _service.AddAsync(s);
            await LoadAsync();
        }

        public async Task UpdateAsync(Subscription s)
        {
            await _service.UpdateAsync(s);
            await LoadAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _service.DeleteAsync(id);
            await LoadAsync();
        }
    }
}
