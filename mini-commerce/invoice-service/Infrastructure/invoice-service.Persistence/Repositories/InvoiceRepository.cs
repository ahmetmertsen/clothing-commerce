using invoice_service.Application.Repositories;
using invoice_service.Domain.Entities;
using invoice_service.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace invoice_service.Persistence.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly InvoiceServiceDbContext _context;

        public InvoiceRepository(InvoiceServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Invoice?> GetByIdAsync(Guid id) => await _context.Invoices
            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(Invoice invoice) => await _context.Invoices.AddAsync(invoice);

        public void Update(Invoice invoice) => _context.Invoices.Update(invoice);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<List<Invoice>> GetByOrderIdAsync(Guid orderId) => await _context.Invoices
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }
}
