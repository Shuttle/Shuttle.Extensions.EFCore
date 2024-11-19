using Microsoft.EntityFrameworkCore;
using Shuttle.Core.Contract;

namespace Shuttle.Extensions.EFCore;

public class DbContextService : IDbContextService
{
    private static readonly Type DbContextType = typeof (DbContext);

    private readonly DbContextCollection _dbContextCollection = new();
    private readonly object _lock = new object();

    public IDisposable Add(DbContext dbContext)
    {
        Guard.AgainstNull(dbContext);

        IDisposable result;

        lock(_lock)
        {
            result = GetDbContextCollection().Add(dbContext);
        }

        return result;
    }

    public bool Contains(Type type)
    {
        lock (_lock)
        {
            return GetDbContextCollection().Contains(type);
        }
    }

    public void Remove(Type type)
    {
        Guard.AgainstNull(type);

        lock(_lock)
        {
            GetDbContextCollection().Remove(type);
        }
    }

    public DbContext Get(Type type)
    {
        lock(_lock)
        {
            return _dbContextCollection.Get(type);
        }
    }

    private DbContextCollection GetDbContextCollection()
    {
        return DbContextScope.Current ?? _dbContextCollection;
    }
}