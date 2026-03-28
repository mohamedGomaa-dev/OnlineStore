using Store.DataAccess.Helpers;
using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories.interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<(IEnumerable<Category> items, int TotalCount)> GetCategoriesAsync(CategoryQuery query);

    }
}
