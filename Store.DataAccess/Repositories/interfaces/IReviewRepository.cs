using Store.DataAccess.Helpers;
using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories.interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<(IEnumerable<Review> Items, int TotalCount)> GetReviewsAsync(ReviewQuery query);
    }
}
