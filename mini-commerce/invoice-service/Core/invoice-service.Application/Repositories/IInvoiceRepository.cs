using invoice_service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice_service.Application.Repositories
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(Guid id);
        Task AddAsync(Invoice invoice);
        void Update(Invoice invoice);
        Task SaveChangesAsync();

        Task<List<Invoice>> GetByOrderIdAsync(Guid orderId);
    }
}
