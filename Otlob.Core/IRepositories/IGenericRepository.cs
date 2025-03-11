namespace Otlob.Core
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetById(int id);
        Task<IReadOnlyList<T>> GetAll();
        Task Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T?> GetAnyByPredicate(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        Task<T?> GetAnyByPredicateWithCustomIncludes(System.Linq.Expressions.Expression<Func<T, bool>> predicate, string customIncludes);
        Task<T?> GetAnyByPredicateWithAllIncludes(System.Linq.Expressions.Expression<Func<T, bool>> predicate, int maxLevel);
    }
}