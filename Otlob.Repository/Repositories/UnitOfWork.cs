using Otlob.Core;
using Otlob.Core.IRepositories;
using Otlob.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangesAsync()
            => await _appDbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
        {
            await _appDbContext.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
