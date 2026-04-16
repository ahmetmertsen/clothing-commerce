using Microsoft.EntityFrameworkCore;
using payment_service.Application.Repositories;
using payment_service.Domain.Entities;
using payment_service.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Persistence.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentServiceDbContext _context;

        public PaymentRepository(PaymentServiceDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetByIdAsync(Guid id) => await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);

        public async Task AddAsync(Payment payment) => await _context.Payments.AddAsync(payment);

        public void Update(Payment payment) => _context.Payments.Update(payment);

        public async Task<List<Payment>> GetByCustomerIdAsync(Guid customerId) => await _context.Payments
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.CreatedDate)
            .ToListAsync();

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
