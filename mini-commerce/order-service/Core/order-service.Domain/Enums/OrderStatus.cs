using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,
        StockReserved,
        PaymentPending,
        Paid,
        Preparing,
        Shipped,
        Delivered,
        Cancelled,
        Failed
    }
}
