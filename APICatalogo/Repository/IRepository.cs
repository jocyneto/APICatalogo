using System.Linq.Expressions;

namespace APICatalogo.Repository;

public interface IRepository<T>
{
    //Retornando um IQueryable, eu posso customizar esta consulta.
    IQueryable<T> Get(); //IQueryable permite fazer chamadas assincronas
    Task<T> GetById(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);

}
