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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<(IEnumerable<User>, int TotalCount)> GetUsersAsync(UserQuery query)
        {
            var users = _context.Users.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                users = users.Where(u => u.Name.Contains(query.Name));
            }
            var totalCount = await users.CountAsync();

            if (!string.IsNullOrWhiteSpace(query.OrderBy))
            {
                if (query.OrderBy.Equals("Name", StringComparison.OrdinalIgnoreCase)) {
                    users = query.IsDescending ? 
                        users.OrderByDescending(u => u.Name) : 
                        users.OrderBy(u => u.Name);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            users = users.Skip(skipNumber).Take(query.PageSize);
            var items=  await users.ToListAsync();
            return (items, totalCount);
        }
    }
}
