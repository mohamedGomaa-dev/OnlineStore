using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.DataAccess.Helpers;
using Store.DataAccess.Repositories.interfaces;
using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories.implementations
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<(IEnumerable<Product> Items, int TotalCount)> GetProductsAsync(ProductQuery query)
        {
            var products = _context.Products.Include(p => p.ProductImages).Include(p => p.Category).AsNoTracking().AsQueryable();


            // check filtering
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                products = products.Where(p => p.Name.Contains(query.Name));
            }

            if (query.CategoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId ==  query.CategoryId.Value);
            }

            if (query.MinPrice.HasValue)
            {
                products = products.Where(p => p.Price >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= query.MaxPrice.Value);
            }

            // get total count after filtering
            var totalCount = await products.CountAsync();

            // check if there is ordering
            if (!string.IsNullOrWhiteSpace(query.OrderBy))
            {
                if (query.OrderBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    products = query.IsDescending ?
                        products.OrderByDescending(u => u.Name) :
                        products.OrderBy(u => u.Name);
                }

                if (query.OrderBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    products = query.IsDescending ?
                        products.OrderByDescending(u => u.Price) :
                        products.OrderBy(u => u.Price);
                }
                if (query.OrderBy.Equals("Category", StringComparison.OrdinalIgnoreCase))
                {
                    products = query.IsDescending ?
                        products.OrderByDescending(u => u.Category.Name) :
                        products.OrderBy(u => u.Category.Name);
                }
            }
            int skipNumber = (query.PageNumber - 1) * query.PageSize;

            products = products.Skip(skipNumber).Take(query.PageSize);
            var items = await products.ToListAsync();

            return (items, totalCount);
        }
    }
}
