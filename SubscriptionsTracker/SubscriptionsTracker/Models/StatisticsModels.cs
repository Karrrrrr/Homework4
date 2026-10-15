using System.Collections.Generic;

namespace SubscriptionsTracker.Models
{
    public class CategoryStat
    {
        public Category Category { get; set; }
        public decimal Amount { get; set; }
        public double Percent { get; set; }
    }

    public class TopSubscription
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
