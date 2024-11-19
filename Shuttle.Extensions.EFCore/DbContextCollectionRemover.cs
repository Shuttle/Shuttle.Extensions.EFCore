namespace Shuttle.Extensions.EFCore;

internal class DbContextCollectionRemover : IDisposable
{
    private readonly DbContextCollection _dbContextCollection;
    private readonly Type _type;

    public DbContextCollectionRemover(DbContextCollection dbContextCollection, Type type)
    {
        _dbContextCollection = dbContextCollection;
        _type = type;
    }

    public void Dispose()
    {
        _dbContextCollection.Remove(_type);
    }
}