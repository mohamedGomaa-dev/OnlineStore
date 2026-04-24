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
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly AppDbContext _context;
        public ReviewRepository(AppDbContext context) : base(context) { 
            _context = context;
        }

        public async Task<(IEnumerable<Review> Items, int TotalCount)> GetReviewsAsync(ReviewQuery query)
        {
            var reviews = _context.Reviews.Include(r => r.User).Include(r => r.Product).AsNoTracking().AsQueryable();
            reviews = reviews.Where(r => r.ProductId == query.ProductId);
            if (query.Rating.HasValue)
            {
                reviews = reviews.Where(r => r.Rating == query.Rating.Value);
            }
            var totalCount = await reviews.CountAsync();

            // check ordering
            if (!string.IsNullOrWhiteSpace(query.OrderBy))
            {
                if (query.OrderBy.Equals("rating", StringComparison.OrdinalIgnoreCase))
                {
                    reviews = query.IsDescending ? reviews.OrderByDescending(r => r.Rating) : reviews.OrderBy(r => r.Rating);
                }
                else if (query.OrderBy.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    reviews = query.IsDescending ? reviews.OrderByDescending(r => r.ReviewDate) : reviews.OrderBy(r => r.ReviewDate);
                }
            }
            // apply pagination: get the skip number = (page number - 1) * page size
            var skipNumber = (query.PageNumber - 1) * query.PageSize;
            var items = await reviews.Skip(skipNumber).Take(query.PageSize).ToListAsync();

            return (items, totalCount);
        }
    }
}
