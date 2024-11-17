using Microsoft.Extensions.Hosting;
using Shuttle.Core.Contract;
using Shuttle.Core.Reflection;
using Shuttle.Core.Threading;

namespace Shuttle.Extensions.EntityFrameworkCore.ThreadDbContextScope;

public class ThreadDbContextScopeHostedService : IHostedService
{
    private const string DbContextScopeStateKey = "Shuttle.Extensions.EntityFrameworkCore.ThreadDbContextScope:DbContextScope";

    private readonly IProcessorThreadPoolFactory _processorThreadPoolFactory;

    public ThreadDbContextScopeHostedService(IProcessorThreadPoolFactory processorThreadPoolFactory)
    {
        _processorThreadPoolFactory = Guard.AgainstNull(processorThreadPoolFactory);

        _processorThreadPoolFactory.ProcessorThreadPoolCreated += OnProcessorThreadPoolCreated;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _processorThreadPoolFactory.ProcessorThreadPoolCreated -= OnProcessorThreadPoolCreated;

        await Task.CompletedTask;
    }

    private void OnProcessorThreadPoolCreated(object? sender, ProcessorThreadPoolCreatedEventArgs e)
    {
        e.ProcessorThreadPool.ProcessorThreadCreated += ProcessorThreadCreated;
    }

    private void ProcessorThreadCreated(object? sender, ProcessorThreadCreatedEventArgs e)
    {
        e.ProcessorThread.ProcessorThreadStarting += ProcessorThreadStarting;
        e.ProcessorThread.ProcessorThreadStopping += ProcessorThreadStopping;
    }

    private void ProcessorThreadStarting(object? sender, ProcessorThreadEventArgs e)
    {
        (sender as ProcessorThread)?.State.Replace(DbContextScopeStateKey, new DbContextScope());
    }

    private void ProcessorThreadStopping(object? sender, ProcessorThreadEventArgs e)
    {
        if (sender is not ProcessorThread processorThread)
        {
            return;
        }

        processorThread.State.Get(DbContextScopeStateKey)?.TryDispose();
        processorThread.State.Remove(DbContextScopeStateKey);

        processorThread.ProcessorThreadStarting -= ProcessorThreadStarting;
        processorThread.ProcessorThreadStopping -= ProcessorThreadStopping;
    }
}