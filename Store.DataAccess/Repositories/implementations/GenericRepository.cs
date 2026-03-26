using Microsoft.EntityFrameworkCore;
using Store.DataAccess.Data;
using Store.DataAccess.Repositories.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Repositories.implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(T item)
        {
            await _context.Set<T>().AddAsync(item);
        }

        public void Delete(T item)
        {
            _context.Set<T>().Remove(item);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().AnyAsync(match);
        }

        public async Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>> match, params string[] includes)
        {
            var items = _context.Set<T>().AsNoTracking().AsQueryable().Where(match);

            if (includes.Count() > 0)
            {
                foreach (var eager in includes)
                {
                    items = items.Include(eager);
                }
            }

            return await items.ToListAsync();   
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(match);
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> match, params string[] includes)
        {
            var item = _context.Set<T>().AsQueryable().Where(match);

            if (includes.Count() > 0)
            {
                foreach (var eager in includes)
                {
                    item = item.Include(eager);
                }
            }

            return await item.FirstOrDefaultAsync();
        }

        public void Update(T item)
        {
            _context.Set<T>().Update(item);
        }
    }
}
