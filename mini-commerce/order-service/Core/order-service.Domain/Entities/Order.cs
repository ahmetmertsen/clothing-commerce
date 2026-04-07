using order_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? FailureReason { get; set; }
        public string? CancelReason { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        private Order() { }

        public Order(Guid customerId, List<OrderItem> orderItems)
        {
            Id = Guid.NewGuid();
            OrderNumber = GenerateOrderNumber();
            CustomerId = customerId;
            Status = OrderStatus.Pending;
            CreatedDate = DateTime.UtcNow;

            foreach (var orderItem in orderItems)
            {
                OrderItems.Add(orderItem);
            }
            TotalAmount = OrderItems.Sum(x => x.LineTotal);
        }


        public void MarkAsStockReserved()
        {
            if (Status != OrderStatus.Pending)
            {
                throw new InvalidOperationException("Yalnızca beklemedeki siparişler için stok ayrılabilir.");
            }

            Status = OrderStatus.StockReserved;
            UpdatedDate = DateTime.UtcNow;
        }

        public void MarkAsPaymentPending()
        {
            if (Status != OrderStatus.StockReserved)
            {
                throw new InvalidOperationException("Yalnızca stoğu ayrılmış siparişler ödeme bekleniyor durumuna alınabilir.");
            }

            Status = OrderStatus.PaymentPending;
            UpdatedDate = DateTime.UtcNow;
        }

        public void MarkAsPaid()
        {
            if (Status != OrderStatus.PaymentPending)
            {
                throw new InvalidOperationException("Yalnızca ödeme sürecindeki siparişler ödendi olarak işaretlenebilir.");
            }

            Status = OrderStatus.Paid;
            UpdatedDate = DateTime.UtcNow;
        }

        public void MarkAsPreparing()
        {
            if (Status != OrderStatus.Paid)
            {
                throw new InvalidOperationException("Yalnızca ödemesi tamamlanmış siparişler hazırlanıyor durumuna alınabilir.");
            }

            Status = OrderStatus.Preparing;
            UpdatedDate = DateTime.UtcNow;
        }

        public void MarkAsShipped()
        {
            if (Status != OrderStatus.Preparing)
            {
                throw new InvalidOperationException("Yalnızca hazırlanan siparişler kargoya verilebilir.");
            }

            Status = OrderStatus.Shipped;
            UpdatedDate = DateTime.UtcNow;
        }

        public void MarkAsDelivered()
        {
            if (Status != OrderStatus.Shipped)
            {
                throw new InvalidOperationException("Yalnızca kargoya verilmiş siparişler teslim edildi olarak işaretlenebilir.");
            }

            Status = OrderStatus.Delivered;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Fail(string reason)
        {
            if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Teslim edilmiş veya iptal edilmiş sipariş başarısız olarak işaretlenemez.");
            }

            Status = OrderStatus.Failed;
            FailureReason = reason;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Cancel(string reason)
        {
            if (Status == OrderStatus.Delivered)
            {
                throw new InvalidOperationException("Teslim edilmiş sipariş iptal edilemez.");
            }

            Status = OrderStatus.Cancelled;
            CancelReason = reason;
            UpdatedDate = DateTime.UtcNow;
        }

        private string GenerateOrderNumber()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        }

    } 
}
