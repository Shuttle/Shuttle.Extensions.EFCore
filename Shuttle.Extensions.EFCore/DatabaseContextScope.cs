namespace Shuttle.Extensions.EFCore;

public class DbContextScope : IDisposable
{
    private static readonly AsyncLocal<Stack<DbContextCollection>> DbContextCollectionStack = new();
    private static readonly AsyncLocal<DbContextCollection?> AmbientData = new();

    public DbContextScope()
    {
        AmbientData.Value = new();
        DbContextCollectionStack.Value ??= new();
        DbContextCollectionStack.Value.Push(AmbientData.Value);
    }

    public static DbContextCollection? Current => AmbientData.Value;

    public void Dispose()
    {
        if (DbContextCollectionStack.Value == null ||
            DbContextCollectionStack.Value.Count == 0)
        {
            AmbientData.Value = null;
            return;
        }

        AmbientData.Value = DbContextCollectionStack.Value?.Pop();
    }
}