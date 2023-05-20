using System.Linq.Expressions;
using Treviso.Domain.Sql.Contexts.Interfaces;
using Treviso.Domain.Sql.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Treviso.Domain.Sql.Repositories;

public abstract class Repository<T> : IRepository<T> where T : class
{
    private readonly ITrevisoContext _trevisoContext;

    public Repository(ITrevisoContext trevisoContext)
    {
        _trevisoContext = trevisoContext;
    }
    public IQueryable<T> GetAll()
    {
        return _trevisoContext.Set<T>();
    }
    public IQueryable<T> GetAllNoTracking()
    {
        return _trevisoContext.Set<T>().AsNoTracking();
    }
    public T? GetSingle(params object?[]? keyValues)
    {
        return _trevisoContext.Set<T>().Find(keyValues);
    }
    public T? GetSingle(Expression<Func<T, bool>> predicate)
    {
        return _trevisoContext.Set<T>().FirstOrDefault(predicate);
    }

    public IQueryable<T> GetMany(Expression<Func<T, bool>> predicate)
    {
        return _trevisoContext.Set<T>().Where(predicate);
    }
    public T? GetSingleNoTracking(Expression<Func<T, bool>> predicate)
    {
        return _trevisoContext.Set<T>().AsNoTracking().FirstOrDefault(predicate);
    }
    public T Add(T entity)
    {
        _trevisoContext.Add(entity);
        return entity;
    }
    public List<T> AddRange(List<T> entities)
    {
        _trevisoContext.AddRange(entities);
        return entities;
    }
    public T Update(T entity, T oldEntity)
    {
        oldEntity = entity;
        _trevisoContext.Update(oldEntity);
        return oldEntity;
    }
    public T Remove(T entity)
    {
        _trevisoContext.Remove(entity);
        return entity;
    }
    public int SaveChanges()
    {
        return _trevisoContext.SaveChanges();
    }
}