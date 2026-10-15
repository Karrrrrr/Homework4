using System;

namespace SubscriptionsTracker.Models
{
    public class Subscription
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public Category Category { get; set; } = Category.Other;
        public Frequency Frequency { get; set; } = Frequency.Monthly;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime NextBillingDate { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
