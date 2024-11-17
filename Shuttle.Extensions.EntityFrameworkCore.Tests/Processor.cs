using Microsoft.EntityFrameworkCore;
using Shuttle.Core.Contract;
using Shuttle.Core.Threading;

namespace Shuttle.Extensions.EntityFrameworkCore.Tests;

public class Processor : IProcessor
{
    private readonly IDbContextFactory<FixtureDbContext> _dbContextFactory;
    private readonly IDbContextService _dbContextService;

    public Processor(IDbContextService dbContextService, IDbContextFactory<FixtureDbContext> dbContextFactory)
    {
        _dbContextService = Guard.AgainstNull(dbContextService);
        _dbContextFactory = Guard.AgainstNull(dbContextFactory);
    }

    public async Task ExecuteAsync(IProcessorThreadContext context, CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await using (var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken))
            using (_dbContextService.Add(dbContext))
            {
                Console.WriteLine($@"[Processor.ExecuteAsync] : managed thread id = {Environment.CurrentManagedThreadId}");
            }

            await Task.Delay(100, cancellationToken);
        }
    }
}