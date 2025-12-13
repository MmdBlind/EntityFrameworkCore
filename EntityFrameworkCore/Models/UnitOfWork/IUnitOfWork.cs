using EntityFrameworkCore.Models.Repository;

namespace EntityFrameworkCore.Models.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepositoryBase<TEntity> BaseRepository<TEntity>() where TEntity : class;
        
        Task Commit();
    }
}
