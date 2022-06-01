namespace OrganisationRegistry.Tests.Shared.TestDataBuilders;

using System;
using FormalFrameworkCategory.Events;

public class FormalFrameworkCategoryCreatedBuilder
{
    public Guid Id { get; private set; }
    public string Name { get;  private set;}

    public FormalFrameworkCategoryCreatedBuilder()
    {
        Id = Guid.NewGuid();
        Name = Id.ToString();
    }

    public FormalFrameworkCategoryCreatedBuilder WithId(Guid id)
    {
        Id = id;
        return this;
    }

    public FormalFrameworkCategoryCreatedBuilder WithName(string formalFramworkCategoryName)
    {
        Name = formalFramworkCategoryName;
        return this;
    }


    public FormalFrameworkCategoryCreated Build()
        => new(
            Id,
            Name);

    public static implicit operator FormalFrameworkCategoryCreated(FormalFrameworkCategoryCreatedBuilder builder)
        => builder.Build();
}
