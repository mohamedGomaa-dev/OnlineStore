using Store.DataAccess.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories.interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetAsync(Expression<Func<T, bool>> match);
        Task<T?> GetAsync(Expression<Func<T, bool>> match, params string[] includes);
        //Task<ICollection<T>> GetAllAsync(params string[] includes); // will not be used for now
        //Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>> match); // will not be used for now
        Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>> match, params string[] includes);

        Task AddAsync(T item);
        void Update(T item);
        void Delete(T item);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> match);
    }
}
