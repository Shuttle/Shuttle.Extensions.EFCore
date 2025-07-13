using System.Diagnostics.CodeAnalysis;

namespace Shuttle.Extensions.EFCore;

public static class RecordNotFoundExtensions
{
    public static T GuardAgainstRecordNotFound<T>([NotNull] this T? entity, object id) where T : class
    {
        if (entity == null)
        {
            throw RecordNotFoundException.For<T>(id);
        }

        return entity;
    }
}