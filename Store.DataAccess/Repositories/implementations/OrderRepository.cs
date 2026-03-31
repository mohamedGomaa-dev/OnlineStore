using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.DataAccess.Repositories.interfaces;
using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories.implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public async Task<Order?> GetOrderByIdWithItemsAsync(int orderId)
        {
           return await _context.Orders.Include(o => o.OrderItems).ThenInclude(i => i.Product).FirstOrDefaultAsync(o => o.Id == orderId);

           
        }
    }
}
