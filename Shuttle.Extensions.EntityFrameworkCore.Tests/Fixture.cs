using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Shuttle.Extensions.EntityFrameworkCore.Tests;

[TestFixture]
public abstract class Fixture
{
    private static readonly IServiceCollection Services = new ServiceCollection();
    protected static IServiceProvider? ServiceProvider;

    [SetUp]
    public void DataAccessSetup()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var fixtureOptions = configuration.GetSection(FixtureOptions.SectionName).Get<FixtureOptions>()!;

        Services.AddSingleton<IConfiguration>(configuration);

        Services.AddOptions<FixtureOptions>().Configure(options =>
        {
            options.ConnectionStringName = fixtureOptions.ConnectionStringName;
            options.MigrationsHistoryTableName = fixtureOptions.MigrationsHistoryTableName;
            options.Schema = fixtureOptions.Schema;
        });

        Services.AddSingleton<IDbContextService, DbContextService>();

        Services.AddDbContextFactory<FixtureDbContext>((provider, builder) =>
        {
            var options = provider.GetRequiredService<IOptions<FixtureOptions>>().Value;
            var connectionString = configuration.GetConnectionString(options.ConnectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"Could not find a connection string called '{options.ConnectionStringName}'.");
            }

            builder.UseSqlServer(connectionString, sqlServerBuilder =>
            {
                sqlServerBuilder.CommandTimeout(300);
                sqlServerBuilder.MigrationsHistoryTable(fixtureOptions.MigrationsHistoryTableName, fixtureOptions.Schema);
            });

            builder.ReplaceService<IMigrationsAssembly, SchemaMigrationsAssembly>();
        });

        ServiceProvider = Services.BuildServiceProvider();

        DbContextFactory = ServiceProvider.GetRequiredService<IDbContextFactory<FixtureDbContext>>();
        DbContextService = ServiceProvider.GetRequiredService<IDbContextService>();

        using (var dbContext = DbContextFactory.CreateDbContext())
        {
            dbContext.Database.Migrate();

            dbContext.FixtureEntities.RemoveRange(dbContext.FixtureEntities.ToList());

            dbContext.SaveChanges();
        }
    }

    public IDbContextFactory<FixtureDbContext> DbContextFactory { get; private set; } = null!;
    public IDbContextService DbContextService { get; private set; } = null!;
}