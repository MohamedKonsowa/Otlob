using Otlob.Core;
using Otlob.Core.IRepositories;
using Otlob.Repository.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Repository.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _appDbContext;
        private Hashtable _repositories = [];
        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repository = new GenericRepository<T>(_appDbContext);
                _repositories.Add(type, repository);
            }

            return _repositories[type] as IGenericRepository<T>;
        }

        public async Task<int> SaveChangesAsync()
            => await _appDbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _appDbContext.DisposeAsync();
    }
}
