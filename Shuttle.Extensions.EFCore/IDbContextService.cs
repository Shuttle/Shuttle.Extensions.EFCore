using Microsoft.EntityFrameworkCore;

namespace Shuttle.Extensions.EFCore;

public interface IDbContextService
{
    IDisposable Add(DbContext dbContext);
    bool Contains(Type type);
    DbContext Get(Type type);
    void Remove(Type type);
}