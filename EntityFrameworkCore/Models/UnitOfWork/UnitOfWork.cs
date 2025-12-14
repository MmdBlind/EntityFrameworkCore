using EntityFrameworkCore.Models.Repository;

namespace EntityFrameworkCore.Models.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public BookShopContext _Context { get; }
        public IBooksRepository _iBooksRepository;
        public UnitOfWork(BookShopContext context, IBooksRepository repository)
        {
            _Context = context;
        }

        public IRepositoryBase<TEntity> BaseRepository<TEntity>() where TEntity : class
        {
            IRepositoryBase<TEntity> repository = new RepositoryBase<TEntity, BookShopContext>(_Context);
            return repository;
        }

        public IBooksRepository BooksRepository
        {
            get
            {
                if (_iBooksRepository == null)
                {
                    _iBooksRepository = new BooksRepository(_Context);
                }
                return _iBooksRepository;
            }
        }

        public async Task Commit()
        {
            await _Context.SaveChangesAsync();
        }


    }
}