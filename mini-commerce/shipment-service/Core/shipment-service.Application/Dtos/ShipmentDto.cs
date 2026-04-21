using shipment_service.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shipment_service.Application.Dtos
{
    public class ShipmentDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string TrackingNumber { get; set; } = null!;
        public string CarrierCompany { get; set; } = null!;
        public ShipmentStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
