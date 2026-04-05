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
    public class ShippingRepository : GenericRepository<Shipping>, IShippingRepository
    {
        private readonly AppDbContext _context;
        public ShippingRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
