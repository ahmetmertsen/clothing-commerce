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
        public DateTime UpdatedDate { get; set; }
        public string? FailureReason { get; set; }
        public string? CancelReason { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

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
            Status = OrderStatus.StockReserved;
            UpdatedDate = DateTime.UtcNow;
        }

        public void MarkAsPaymentPending()
        {
            Status = OrderStatus.PaymentPending;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Confirm()
        {
            Status = OrderStatus.Confirmed;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Fail(string reason)
        {
            Status = OrderStatus.Failed;
            FailureReason = reason;
            UpdatedDate = DateTime.UtcNow;
        }

        public void Cancel(string reason)
        {
            Status = OrderStatus.Cancelled;
            CancelReason = reason;
            UpdatedDate = DateTime.UtcNow;
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}";
        }
    } 
}
