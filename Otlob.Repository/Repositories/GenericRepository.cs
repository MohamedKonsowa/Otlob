using Microsoft.EntityFrameworkCore.Metadata;
using Otlob.Core;
using Otlob.Repository.Context;
using System.Linq.Expressions;

namespace Otlob.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _appDbContext;

        public GenericRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task Add(T entity)
            => await _appDbContext.AddAsync(entity);

        public async Task<T?> GetById(int id) 
            => await _appDbContext.FindAsync<T>(id);

        public Task<IReadOnlyList<T>?> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(T entity) 
            => _appDbContext.Update(entity);

        public void Delete(T entity)
        {
            _appDbContext.Remove(entity);
        }
        public async Task<T?> GetAnyByPredicate(Expression<Func<T, bool>> predicate)
        {
           return await _appDbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> GetAnyByPredicateWithCustomIncludes(Expression<Func<T, bool>> predicate, string customIncludes)
        {
            IQueryable<T>? query = _appDbContext.Set<T>().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(customIncludes))
            {
                foreach (var include in customIncludes.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> GetAnyByPredicateWithAllIncludes(Expression<Func<T, bool>> predicate, int maxLevel)
        {
            if (maxLevel < 0)
                throw new ArgumentException("Max level cannot be negative", nameof(maxLevel));

            IQueryable<T> query = _appDbContext.Set<T>().AsNoTracking();

            var entityType = _appDbContext.Model.FindEntityType(typeof(T));
            if (entityType != null)
            {
                foreach (var navigation in entityType.GetNavigations())
                {
                    query = IncludeNavigationProperties(query, navigation, maxLevel, 0);
                }
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        private IQueryable<T> IncludeNavigationProperties(IQueryable<T> query, INavigation navigation, int maxLevel, int currentLevel)
        {
            if (currentLevel >= maxLevel)
                return query;

            var included = query.Include(navigation.Name);

            var targetType = navigation.TargetEntityType;
            if (targetType != null && currentLevel < maxLevel - 1)
            {
                foreach (var nestedNavigation in targetType.GetNavigations())
                {
                    // Create a string path for nested navigation
                    var nestedPath = $"{navigation.Name}.{nestedNavigation.Name}";
                    query = query.Include(nestedPath);
                }
            }

            return included;
        }
    }
}