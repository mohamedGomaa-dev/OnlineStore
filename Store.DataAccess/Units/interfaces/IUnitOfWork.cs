using Store.DataAccess.Repositories.interfaces;
using Store.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Units.interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IUserRepository Users { get; }
        Task<int> CommitChanges();
    }
}
