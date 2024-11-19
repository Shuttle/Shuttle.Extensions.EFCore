using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using NUnit.Framework;
using Shuttle.Core.Threading;
using Shuttle.Extensions.EFCore.ThreadDbContextScope;

namespace Shuttle.Extensions.EFCore.Tests;

[TestFixture]
public class ThreadDatabaseContextScopeHostedServiceFixture : Fixture
{
    [Test]
    public async Task Should_be_able_to_access_existing_database_context_scope_async()
    {
        var serviceProvider = Core.Contract.Guard.AgainstNull(ServiceProvider);

        _ = serviceProvider.GetServices<IHostedService>().OfType<ThreadDbContextScopeHostedService>().Single();

        var processorThreadPoolFactory = serviceProvider.GetRequiredService<IProcessorThreadPoolFactory>();

        var processorFactory = new Mock<IProcessorFactory>();

        processorFactory.Setup(m => m.Create()).Returns(new Processor(serviceProvider.GetRequiredService<IDbContextService>(), serviceProvider.GetRequiredService<IDbContextFactory<FixtureDbContext>>()));

        var processorThreadPool = processorThreadPoolFactory.Create("test", 3, processorFactory.Object, new());

        await processorThreadPool.StartAsync();

        await Task.Delay(2000);

        await processorThreadPool.StopAsync();
    }
}