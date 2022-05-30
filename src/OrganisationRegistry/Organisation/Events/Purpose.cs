namespace OrganisationRegistry.Organisation.Events;

using System;

public class Purpose
{
    public Guid Id { get; }
    public string Name { get; }

    public Purpose(
        Guid id,
        string name)
    {
        Id = id;
        Name = name;
    }
}