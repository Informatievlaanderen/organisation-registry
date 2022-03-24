namespace OrganisationRegistry.Tests.Shared.TestDataBuilders
{
    using System;
    using FormalFramework;
    using FormalFramework.Events;

    public class FormalFrameworkCreatedBuilder
    {
        public FormalFrameworkId Id { get; }
        public string Name { get; }
        public string FormalFrameworkCategoryName { get; }
        public Guid FormalFrameworkCategoryId { get; }
        public string Code { get; }

        public FormalFrameworkCreatedBuilder(Guid formalFrameworkCategoryId, string formalFrameworkCategoryName)
        {
            Id = new FormalFrameworkId(Guid.NewGuid());
            Name = Id.ToString();
            Code = Id.ToString();
            FormalFrameworkCategoryId = formalFrameworkCategoryId;
            FormalFrameworkCategoryName = formalFrameworkCategoryName;
        }

        public FormalFrameworkCreated Build()
            => new(
                Id,
                Name,
                Code,
                FormalFrameworkCategoryId,
                FormalFrameworkCategoryName);

        public static implicit operator FormalFrameworkCreated(FormalFrameworkCreatedBuilder builder)
            => builder.Build();
    }
}
