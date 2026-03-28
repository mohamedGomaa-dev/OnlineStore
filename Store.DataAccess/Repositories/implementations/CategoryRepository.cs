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
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) : base(context) 
        {
            _context = context;
        }
        public async Task<(IEnumerable<Category> items, int TotalCount)> GetCategoriesAsync(CategoryQuery query)
        {
            var categories = _context.Categories.AsNoTracking().AsQueryable();

            // check the filtering
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                categories = categories.Where(c => c.Name.Contains(query.Name));
            }

            // get total count of categories that apply to the filtering
            int totalCount = await categories.CountAsync();

            // check the ordering
            if (!string.IsNullOrWhiteSpace(query.OrderBy))
            {
                if (query.OrderBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    categories = query.IsDescending ? categories.OrderByDescending(c => c.Name) :
                        categories.OrderBy(c => c.Name);
                }
            }

            // get the skip number for the pagination
            int skipNumber = (query.PageNumber - 1) * query.PageSize;
            categories = categories.Skip(skipNumber).Take(query.PageSize);
            
            var items = await categories.ToListAsync();

            return (items, totalCount);
        }
    }
}
