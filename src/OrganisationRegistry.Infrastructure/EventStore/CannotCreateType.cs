namespace OrganisationRegistry.Infrastructure.EventStore;

using System;

public class CannotCreateType : Exception
{
    public CannotCreateType(string typeName) : base($"Cannot create type {typeName}")
    {
    }
}
