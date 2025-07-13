namespace Shuttle.Extensions.EFCore;

public class RecordNotFoundException : Exception
{
    public RecordNotFoundException(string message) : base(message)
    {
    }

    public static RecordNotFoundException For(string name, object id)
    {
        return new RecordNotFoundException(string.Format(Resources.RecordIdNotFoundException, name ?? "(unknown)", id ?? "(null)"));
    }

    public static RecordNotFoundException For<T>(object id)
    {
        return For(typeof(T).Name, id);
    }
}