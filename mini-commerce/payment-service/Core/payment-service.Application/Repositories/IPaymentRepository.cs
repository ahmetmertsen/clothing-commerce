using payment_service.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment_service.Application.Repositories
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id);
        Task AddAsync(Payment payment);
        void Update(Payment payment);

        Task<List<Payment>> GetByCustomerIdAsync(Guid customerId);
        Task SaveChangesAsync();
    }
}
