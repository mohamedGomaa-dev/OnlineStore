using Store.DataAccess.Data;
using Store.DataAccess.Repositories.implementations;
using Store.DataAccess.Repositories.interfaces;
using Store.DataAccess.Units.interfaces;
using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Units.implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            ProductImages = new ProductImageRepository(_context);
            Orders = new OrderRepository(_context);
            Payments = new PaymentRepository(_context);
            Shippings = new ShippingRepository(_context);
            Reviews = new ReviewRepository(_context);

        }
        public IUserRepository Users { get; private set; }
        public ICategoryRepository Categories { get; private set; }

        public IProductRepository Products { get; private set; }

        public IProductImageRepository ProductImages { get; private set; }

        public IOrderRepository Orders { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public IShippingRepository Shippings { get; private set; }
        public IReviewRepository Reviews { get; private set; }

        public async Task<int> CommitChanges()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
