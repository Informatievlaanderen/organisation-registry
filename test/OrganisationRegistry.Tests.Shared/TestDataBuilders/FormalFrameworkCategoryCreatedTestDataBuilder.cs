namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using FormalFrameworkCategory.Events;

    public class FormalFrameworkCategoryCreatedTestDataBuilder
    {
        public Guid Id { get; }
        public string Name { get; }

        public FormalFrameworkCategoryCreatedTestDataBuilder()
        {
            Id = Guid.NewGuid();
            Name = Id.ToString();
        }

        public FormalFrameworkCategoryCreated Build()
            => new FormalFrameworkCategoryCreated(
                Id,
                Name);
    }
}
