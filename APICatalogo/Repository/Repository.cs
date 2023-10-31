using APICatalogo.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace APICatalogo.Repository;

public class Repository<T> : IRepository<T> where T : class //Restringe T onde só pode ser uma classe
{
    protected AppDbContext _context;

    public Repository(AppDbContext context)
    {
        this._context = context;
    }
    public IQueryable<T> Get()
    {
        return this._context.Set<T>().AsNoTracking();
    }
    public async Task<T> GetById(Expression<Func<T, bool>> predicate)
    {
        return await this._context.Set<T>().SingleOrDefaultAsync(predicate);
    }

    public void Add(T entity)
    {
        this._context.Add(entity);
    }

    public void Delete(T entity)
    {
        this._context.Remove(entity);
    }

    public void Update(T entity)
    {
        this._context.Entry(entity).State = EntityState.Modified;
        this._context.Update(entity);
    }
}
