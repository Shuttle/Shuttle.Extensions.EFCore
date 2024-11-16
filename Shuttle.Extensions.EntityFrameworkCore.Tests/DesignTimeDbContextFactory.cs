using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Shuttle.Extensions.EntityFrameworkCore.Tests;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FixtureDbContext>
{
    public FixtureDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var fixtureOptions = configuration.GetSection(FixtureOptions.SectionName).Get<FixtureOptions>()!;

        var optionsBuilder = new DbContextOptionsBuilder<FixtureDbContext>();

        optionsBuilder
            .UseSqlServer(configuration.GetConnectionString(fixtureOptions.ConnectionStringName),
                builder => builder.MigrationsHistoryTable(fixtureOptions.MigrationsHistoryTableName, fixtureOptions.Schema));

        optionsBuilder.ReplaceService<IMigrationsAssembly, SchemaMigrationsAssembly>();

        return new(Options.Create(fixtureOptions), optionsBuilder.Options);
    }
}