namespace OrganisationRegistry.Infrastructure.Domain.Exception;

using System;

public class AggregateInstanceNotConstructed : Exception
{
    public AggregateInstanceNotConstructed(Type type)
        : base($"{type.FullName} could not be constructed") { }
}
