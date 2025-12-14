using EntityFrameworkCore.Models.Repository;

namespace EntityFrameworkCore.Models.UnitOfWork
{
    public interface IUnitOfWork
    {
        public BookShopContext _Context { get; }

        IBooksRepository BooksRepository { get; }

        IRepositoryBase<TEntity> BaseRepository<TEntity>() where TEntity : class;

        Task Commit();
    }
}
