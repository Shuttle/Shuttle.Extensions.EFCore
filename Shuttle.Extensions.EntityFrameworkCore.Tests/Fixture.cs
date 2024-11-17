using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shuttle.Core.Threading;
using Shuttle.Extensions.EntityFrameworkCore.ThreadDbContextScope;

namespace Shuttle.Extensions.EntityFrameworkCore.Tests;

[TestFixture]
public abstract class Fixture
{
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

        Services
            .AddSingleton<IDbContextService, DbContextService>()
            .AddSingleton<IProcessorThreadPoolFactory, ProcessorThreadPoolFactory>()
            .AddThreadDbContextScope()
            .AddDbContextFactory<FixtureDbContext>(builder =>
            {
                var connectionString = configuration.GetConnectionString(fixtureOptions.ConnectionStringName);

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new ArgumentException($"Could not find a connection string called '{fixtureOptions.ConnectionStringName}'.");
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

    private static readonly IServiceCollection Services = new ServiceCollection();
    protected static IServiceProvider? ServiceProvider;

    public IDbContextFactory<FixtureDbContext> DbContextFactory { get; private set; } = null!;
    public IDbContextService DbContextService { get; private set; } = null!;
}