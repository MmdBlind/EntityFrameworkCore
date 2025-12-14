using System.Linq.Expressions;

namespace EntityFrameworkCore.Models.Repository
{
    public interface IRepositoryBase<TEntity>
    {
        Task<IEnumerable<TEntity>> FindAllAsync();

        IEnumerable<TEntity> FindAll();

        Task<TEntity> FindById(object id);

        Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params Expression<Func<TEntity, object>>[] includes);

        Task Create(TEntity entity);

        void Update(TEntity entity);

        void Delete(TEntity entity);

        Task CreateRange(IEnumerable<TEntity> entities);

        void UpdateRange(IEnumerable<TEntity> entities);

        void DeleteRange(IEnumerable<TEntity> entities);
    }
}
