namespace OrganisationRegistry.Infrastructure.Domain.Factories;

using System;
using Exception;

internal static class AggregateFactory
{
    public static T CreateAggregate<T>()
    {
        try
        {
            var maybeInstance = (T?)Activator.CreateInstance(typeof(T), true);
            if (maybeInstance is { } instance)
                return instance;

            throw new AggregateInstanceNotConstructed(typeof(T));
        }
        catch (MissingMethodException)
        {
            throw new MissingParameterLessConstructor(typeof(T));
        }
    }
}
