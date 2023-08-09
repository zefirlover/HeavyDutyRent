using System.Linq.Expressions;
using Domain.Interfaces;

namespace Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;
    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
    }
    
    public IEnumerable<T> FindBy(Expression<Func<T, bool>> expression)
    {
        return _dbContext.Set<T>().Where(expression);
    }
    
    public IEnumerable<T> GetAll()
    {
        return _dbContext.Set<T>().ToList();
    }
    
    public T GetById(int id)
    {
        return _dbContext.Set<T>().Find(id);
    }
    
    public void Remove(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
    }
    
    public void RemoveRange(IEnumerable<T> entities)
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }
}