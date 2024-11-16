namespace Shuttle.Extensions.EntityFrameworkCore.Tests;

public class FixtureOptions
{
    public const string SectionName = "Shuttle:Extensions:EntityFrameworkCore";

    public string ConnectionStringName { get; set; } = string.Empty;
    public string Schema { get; set; } = "dbo";
    public string MigrationsHistoryTableName { get; set; } = "__EFMigrationsHistory";
}