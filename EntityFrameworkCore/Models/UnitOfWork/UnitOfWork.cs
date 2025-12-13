using EntityFrameworkCore.Models.Repository;

namespace EntityFrameworkCore.Models.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public BookShopContext _Context { get; }
        public UnitOfWork(BookShopContext context)
        {
            _Context = context;
        }

        public IRepositoryBase<TEntity> BaseRepository<TEntity>() where TEntity : class
        {
            IRepositoryBase<TEntity> repository = new RepositoryBase<TEntity, BookShopContext>(_Context);
            return repository;
        }

        public async Task Commit()
        {
            await _Context.SaveChangesAsync();
        }


    }
}