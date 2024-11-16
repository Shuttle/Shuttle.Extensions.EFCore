using Microsoft.EntityFrameworkCore;
using Shuttle.Core.Contract;

namespace Shuttle.Extensions.EntityFrameworkCore;

public class DbContextService : IDbContextService
{
    private static readonly Type DbContextType = typeof (DbContext);

    private readonly DbContextCollection _dbContextCollection = new();
    private readonly SemaphoreSlim _lock = new(1, 1);

    public IDisposable Add(DbContext dbContext)
    {
        Guard.AgainstNull(dbContext);

        _lock.Wait();

        IDisposable result;

        try
        {
            result = GetDbContextCollection().Add(dbContext);
        }
        finally
        {
            _lock.Release();
        }

        return result;
    }

    public void Remove(Type type)
    {
        Guard.AgainstNull(type);

        _lock.Wait();

        try
        {
            GetDbContextCollection().Remove(type);
        }
        finally
        {
            _lock.Release();
        }
    }

    public DbContext Get(Type type)
    {
        return _dbContextCollection.Get(type);
    }

    private DbContextCollection GetDbContextCollection()
    {
        return DbContextScope.Current ?? _dbContextCollection;
    }
}