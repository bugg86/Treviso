using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Treviso.Domain.Sql.Contexts.Interfaces;

public interface ITrevisoContext
{
    public DbSet<T> Set<T>() where T : class;
    public EntityEntry<T> Add<T>(T entity) where T : class;
    public EntityEntry<T> Update<T>(T entity) where T : class;
    public EntityEntry<T> Remove<T>(T entity) where T : class;
    public int SaveChanges();
    public void AddRange(IEnumerable<object> entities);
}