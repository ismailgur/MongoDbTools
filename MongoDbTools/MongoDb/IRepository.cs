using MongoDB.Driver;
using System.Linq.Expressions;
using static MongoDbTools.MongoDb.MongoDbEnums;

namespace MongoDbTools.MongoDb
{
    public interface IRepository<T, in TKey> where T : class, IEntity<TKey>, new() where TKey : IEquatable<TKey>
    {
        IQueryable<T> Get(Expression<Func<T, bool>> predicate = null);
        
        Task<T> GetAsync(Expression<Func<T, bool>> predicate);
        
        Task<T> GetByIdAsync(TKey id);
        
        Task<T> AddAsync(T entity);
        
        Task<bool> AddRangeAsync(IEnumerable<T> entities);
        
        Task<T> UpdateAsync(TKey id, T entity);
        
        Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> predicate);
        
        Task<T> DeleteAsync(T entity);
        
        Task<T> DeleteAsync(TKey id);

        Task<T> DeleteAsync(Expression<Func<T, bool>> predicate);

        IFindFluent<T, T> Find(FilterDefinition<T> filter);

        Task<Tuple<List<T>?, long>> Paging(int page, int pageSize
            , string orderField = ""
            , OrderByEnum orderBy = OrderByEnum.Asc
            , string searchField = ""
            , string searchTerm = ""
            , CancellationToken? cancellationToken = null);
    }
}