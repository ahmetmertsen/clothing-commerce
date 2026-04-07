using Microsoft.EntityFrameworkCore;
using order_service.Application.Repositories;
using order_service.Domain.Entities;
using order_service.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace order_service.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderServiceDbContext _context;

        public OrderRepository(OrderServiceDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync() => await _context.Orders.ToListAsync();

        public async Task<Order?> GetByIdAsync(Guid id) => await _context.Orders.FindAsync(id);

        public async Task AddAsync(Order order) => await _context.Orders.AddAsync(order);

        public void Update(Order order) => _context.Orders.Update(order);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        public async Task<Order?> GetByIdWithItemsAsync(Guid id) => 
            await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<Order?> GetByOrderNumberWithItemsAsync(string orderNumber) =>
            await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

        public async Task<List<Order>> GetByCustomerIdWithItemsAsync(Guid customerId) =>
            await _context.Orders
            .Include(o => o.OrderItems)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedDate)
            .ToListAsync();

    }
}
