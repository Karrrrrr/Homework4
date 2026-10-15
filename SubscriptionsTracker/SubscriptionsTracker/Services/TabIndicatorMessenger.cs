using System;

namespace SubscriptionsTracker.Services
{
    public static class TabIndicatorMessenger
    {
        public static event Action<string> SetActiveTab;

        public static void Send(string tab)
        {
            SetActiveTab?.Invoke(tab);
        }
    }
}
