using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shuttle.Core.Contract;
using Shuttle.Extensions.EFCore.Tests.Models;

namespace Shuttle.Extensions.EFCore.Tests;

public class FixtureDbContext : DbContext, IDbContextSchema
{
    private readonly FixtureOptions _fixtureOptions;


    public FixtureDbContext(IOptions<FixtureOptions> sqlServerStorageOptions, DbContextOptions<FixtureDbContext> options) : base(options)
    {
        _fixtureOptions = Guard.AgainstNull(Guard.AgainstNull(sqlServerStorageOptions).Value);
    }

    public DbSet<FixtureEntity> FixtureEntities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_fixtureOptions.Schema);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.SetTableName(entityType.DisplayName());
        }
    }

    public string Schema => _fixtureOptions.Schema;
}