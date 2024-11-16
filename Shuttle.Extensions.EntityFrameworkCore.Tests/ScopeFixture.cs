using System.Transactions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Shuttle.Extensions.EntityFrameworkCore.Tests;

public class ScopeFixture : Fixture
{
    private async Task GetRowsAsync(int depth)
    {
        if (depth < 5)
        {
            await GetRowsAsync(depth + 1);
        }

        _ = await DbContextService.Get<FixtureDbContext>().FixtureEntities.ToListAsync();
    }

    [Test]
    public async Task Should_be_able_to_create_nested_database_context_scopes_async()
    {
        using (new DbContextScope())
        await using (var dbContextOuter = await DbContextFactory.CreateDbContextAsync())
        {
            var count = await dbContextOuter.FixtureEntities.CountAsync();

            Assert.That(count, Is.Zero);

            await dbContextOuter.FixtureEntities.AddAsync(new()
            {
                Name = "outer"
            });

            await dbContextOuter.SaveChangesAsync();

            count = await dbContextOuter.FixtureEntities.CountAsync();

            Assert.That(count, Is.EqualTo(1));

            using (new DbContextScope())
            using (new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            await using (var dbContextInner = await DbContextFactory.CreateDbContextAsync())
            {
                await dbContextInner.FixtureEntities.AddAsync(new()
                {
                    Name = "inner"
                });

                await dbContextInner.SaveChangesAsync();

                count = await dbContextInner.FixtureEntities.CountAsync();

                Assert.That(count, Is.EqualTo(2));
            }

            count = await dbContextOuter.FixtureEntities.CountAsync();

            Assert.That(count, Is.EqualTo(1));
        }

        await using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        {
            var count = await dbContext.FixtureEntities.CountAsync();

            Assert.That(count, Is.EqualTo(1));
        }
    }

    [Test]
    public void Should_be_able_to_use_the_different_database_context_for_separate_tasks_async()
    {
        List<Task> tasks = new();

        for (var i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                using (new DbContextScope())
                await using (var dbContext = await DbContextFactory.CreateDbContextAsync())
                {
                    _ = await dbContext.FixtureEntities.ToListAsync();
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }

    [Test]
    public void Should_be_able_to_use_the_different_database_context_for_separate_threads_async()
    {
        List<Thread> threads = new();

        for (var i = 0; i < 10; i++)
        {
            using (ExecutionContext.SuppressFlow())
            {
                threads.Add(new(() =>
                {
                    Task.Run(async () =>
                    {
                        using (new DbContextScope())
                        await using (var dbContext = await DbContextFactory.CreateDbContextAsync())
                        {
                            await dbContext.FixtureEntities.ToListAsync();
                        }
                    }).Wait();
                }));
            }
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    [Test]
    public async Task Should_be_able_to_use_the_same_database_context_across_synchronized_tasks_async()
    {
        await using (var dbContext = await DbContextFactory.CreateDbContextAsync())
        using (DbContextService.Add(dbContext))
        {
            await GetRowsAsync(0);
        }
    }
}