using Microsoft.EntityFrameworkCore;
using Shuttle.Core.Contract;

namespace Shuttle.Extensions.EntityFrameworkCore;

public static class DbContextServiceExtensions
{
    public static void Remove(this IDbContextService dbContextService, DbContext dbContext)
    {
        Guard.AgainstNull(dbContextService).Remove(Guard.AgainstNull(dbContext).GetType());
    }

    public static void Remove<T>(this IDbContextService dbContextService, DbContext dbContext) where T: DbContext
    {
        Guard.AgainstNull(dbContextService).Remove(typeof(T));
    }

    public static T Get<T>(this IDbContextService dbContextService) where T : DbContext
    {
        return (T)Guard.AgainstNull(dbContextService).Get(typeof(T));
    }

    public static bool Contains<T>(this IDbContextService dbContextService) where T : DbContext
    {
        return Guard.AgainstNull(dbContextService).Contains(typeof(T));
    }
}