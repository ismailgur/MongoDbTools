using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;
using static MongoDbTools.MongoDb.MongoDbAttributes;
using static MongoDbTools.MongoDb.MongoDbEnums;

namespace MongoDbTools.MongoDb
{
    public class MongoDbRepositoryBase<T>  where T : MongoDbEntity, new()
    {
        private readonly IMongoCollection<T> Collection;
        private readonly MongoDbSettings settings;

        public MongoDbRepositoryBase(MongoDbSettings settings)
        {
            this.settings = settings;
            var client = new MongoClient(this.settings.ConnectionString);
            var db = client.GetDatabase(this.settings.Database);

            var collectionName = (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault()
                                as BsonCollectionAttribute)?.CollectionName
                                ?? typeof(T).Name.ToLowerInvariant();

            this.Collection = db.GetCollection<T>(collectionName);
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? Collection.AsQueryable()
                : Collection.AsQueryable().Where(predicate);
        }


        public Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return Collection.Find(predicate).FirstOrDefaultAsync();
        }

        public Task<T> GetByIdAsync(string id)
        {
            return Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            var options = new InsertOneOptions { BypassDocumentValidation = false };
            await Collection.InsertOneAsync(entity, options);
            return entity;
        }

        public async Task<bool> AddRangeAsync(IEnumerable<T> entities)
        {
            var options = new BulkWriteOptions { IsOrdered = false, BypassDocumentValidation = false };
            return (await Collection.BulkWriteAsync((IEnumerable<WriteModel<T>>)entities, options)).IsAcknowledged;
        }

        public async Task<T> UpdateAsync(string id, T entity)
        {
            return await Collection.FindOneAndReplaceAsync(x => x.Id == id, entity);
        }

        public async Task<T> UpdateAsync(T entity, Expression<Func<T, bool>> predicate)
        {
            return await Collection.FindOneAndReplaceAsync(predicate, entity);
        }

        public async Task<T> DeleteAsync(T entity)
        {
            return await Collection.FindOneAndDeleteAsync(x => x.Id == entity.Id);
        }

        public async Task<T> DeleteAsync(string id)
        {
            return await Collection.FindOneAndDeleteAsync(x => x.Id == id);
        }

        public async Task<T> DeleteAsync(Expression<Func<T, bool>> filter)
        {
            return await Collection.FindOneAndDeleteAsync(filter);
        }

        public IFindFluent<T, T> Find(FilterDefinition<T> filter)
        {
            return Collection.Find(filter);
        }

        public async Task<Tuple<List<T>?, long>> Paging(int page, int pageSize
            , string orderField = ""
            , OrderByEnum orderBy = OrderByEnum.Asc
            , string searchField = ""
            , string searchTerm = ""
            , CancellationToken? cancellationToken = null)
        {
            #region filter build

            FilterDefinition<T> filterDefinition;

            filterDefinition = Builders<T>.Filter.Eq(x => searchField, searchTerm);

            if (string.IsNullOrEmpty(searchField) || string.IsNullOrEmpty(searchTerm))
            {
                filterDefinition = Builders<T>.Filter.Empty;
            }
            else
            {
                filterDefinition = Builders<T>.Filter.Eq(x => searchField, searchTerm);
            }

            #endregion


            #region sort build

            var sort = Builders<T>.Sort.Ascending(orderField);

            SortDefinition<T> sortDefinition;

            if (string.IsNullOrEmpty(orderField))
                orderField = "Id";


            switch (orderBy)
            {
                case OrderByEnum.Asc:
                    sortDefinition = Builders<T>.Sort.Ascending(orderField);
                    break;
                case OrderByEnum.Desc:
                    sortDefinition = Builders<T>.Sort.Descending(orderField);
                    break;
            }

            #endregion



            var data = await Collection.Find(filterDefinition)
                                       .Sort(sort)
                                       .Skip((page - 1) * pageSize)
                                       .Limit(pageSize)
                                       .ToListAsync();

            var totalCount = await Collection.CountDocumentsAsync(filterDefinition, cancellationToken: cancellationToken ?? CancellationToken.None);

            return new Tuple<List<T>?, long>(data, totalCount);
        }
    }
}