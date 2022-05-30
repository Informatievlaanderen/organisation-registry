namespace OrganisationRegistry.FormalFrameworkCategory.Events;

using System;

public class FormalFrameworkCategoryCreated : BaseEvent<FormalFrameworkCategoryCreated>
{
    public Guid FormalFrameworkCategoryId => Id;

    public string Name { get; }

    public FormalFrameworkCategoryCreated(
        Guid formalFrameworkCategoryId,
        string name)
    {
        Id = formalFrameworkCategoryId;
        Name = name;
    }
}