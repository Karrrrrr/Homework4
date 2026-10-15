using System;
using System.Threading.Tasks;
using SubscriptionsTracker.Models;
using SubscriptionsTracker.Services;

namespace SubscriptionsTracker.ViewModels
{
    public class SubscriptionViewModel : BindableObject
    {
        private readonly SubscriptionService _service;

        public Subscription Model { get; private set; }

        public SubscriptionViewModel(SubscriptionService service)
        {
            _service = service;
            Model = new Subscription();
        }

        public void Load(Subscription model)
        {
            Model = model;
            OnPropertyChanged(nameof(Model));
        }

        public Task SaveAsync()
        {
            if (Model.Id == Guid.Empty)
                return _service.AddAsync(Model);
            return _service.UpdateAsync(Model);
        }

        public Task DeleteAsync()
        {
            return _service.DeleteAsync(Model.Id);
        }
    }
}
