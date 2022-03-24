namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using FormalFrameworkCategory.Events;

    public class FormalFrameworkCategoryCreatedBuilder
    {
        public Guid Id { get; }
        public string Name { get; }

        public FormalFrameworkCategoryCreatedBuilder()
        {
            Id = Guid.NewGuid();
            Name = Id.ToString();
        }

        public FormalFrameworkCategoryCreated Build()
            => new(
                Id,
                Name);

        public static implicit operator FormalFrameworkCategoryCreated(FormalFrameworkCategoryCreatedBuilder builder)
            => builder.Build();
    }
}
