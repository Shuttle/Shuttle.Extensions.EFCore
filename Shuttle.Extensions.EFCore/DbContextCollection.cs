using Microsoft.EntityFrameworkCore;

namespace Shuttle.Extensions.EFCore;

public class DbContextCollection
{
    private readonly Dictionary<Type, DbContext> _dbContexts = new();

    public IEnumerable<DbContext> GetDbContexts() => _dbContexts.Values.ToList().AsReadOnly();

    public IDisposable Add(DbContext dbContext)
    {
        var type = dbContext.GetType();

        if (!_dbContexts.TryAdd(type, dbContext))
        {
            throw new(string.Format(Resources.DuplicateDbContextException, type.FullName));
        }

        return new DbContextCollectionRemover(this, dbContext.GetType());
    }

    public void Remove(Type type)
    {
        if (!_dbContexts.ContainsKey(type))
        {
            throw new InvalidOperationException(string.Format(Resources.DbContextNotFoundException, type.FullName));
        }

        _dbContexts.Remove(type);
    }

    public DbContext Get(Type type)
    {
        if (!_dbContexts.TryGetValue(type, out var context))
        {
            throw new InvalidOperationException(string.Format(Resources.DbContextNotFoundException, type.FullName));
        }

        return context;
    }

    public bool Contains(Type type)
    {
        return _dbContexts.ContainsKey(type);
    }
}