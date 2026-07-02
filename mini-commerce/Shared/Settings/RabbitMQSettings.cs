using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Settings
{
    public static class RabbitMQSettings
    {
        public const string CheckoutStateMachineQueue = "checkout-state-machine-queue";
        public const string NotificationRequestedQueue = "notification-requested-queue";
        public const string AuthCustomerEventsQueue = "auth-customer-events-queue";
    }
}
