using Microsoft.EntityFrameworkCore;

namespace Shuttle.Extensions.EntityFrameworkCore;

public interface IDbContextService
{
    IDisposable Add(DbContext dbContext);
    void Remove(Type type);
    DbContext Get(Type type);
}