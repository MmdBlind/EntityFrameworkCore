using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EntityFrameworkCore.Models.Repository
{
    public class RepositoryBase<TEntity> where TEntity : class
    {
        protected BookShopContext _Context;
        public RepositoryBase(BookShopContext Context) 
        {
            _Context = Context;
        }
        public async Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return await _Context.Set<TEntity>().ToListAsync(); 
        }
        public async Task<TEntity> FindById(object id)
        { 
            return await _Context.Set<TEntity>().FindAsync(id);
        }
        public async Task<IEnumerable<TEntity>> FindByConditionAsync(Expression<Func<TEntity,bool>> filter=null,Func<IQueryable<TEntity>,IOrderedQueryable> orderBy=null,)
        { 
        }
    }
}
