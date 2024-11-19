using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Shuttle.Core.Contract;

namespace Shuttle.Extensions.EFCore;

public class SchemaMigrationsAssembly : MigrationsAssembly
{
    private readonly DbContext _context;

    public SchemaMigrationsAssembly(ICurrentDbContext currentContext, IDbContextOptions options, IMigrationsIdGenerator idGenerator, IDiagnosticsLogger<DbLoggerCategory.Migrations> logger)
        : base(currentContext, options, idGenerator, logger)
    {
        _context = Guard.AgainstNull(currentContext.Context);
    }

    public override Migration CreateMigration(TypeInfo migrationClass, string activeProvider)
    {
        ArgumentNullException.ThrowIfNull(activeProvider);

        var hasCtorWithSchema = migrationClass.GetConstructor(new[] { typeof(IDbContextSchema) }) != null;

        if (hasCtorWithSchema && _context is IDbContextSchema schema)
        {
            var instance = (Migration)Activator.CreateInstance(migrationClass.AsType(), schema)!;
            instance.ActiveProvider = activeProvider;
            return instance;
        }

        return base.CreateMigration(migrationClass, activeProvider);
    }
}