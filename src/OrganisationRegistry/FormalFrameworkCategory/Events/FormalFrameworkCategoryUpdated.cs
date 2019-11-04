namespace OrganisationRegistry.FormalFrameworkCategory.Events
{
    using System;

    public class FormalFrameworkCategoryUpdated : BaseEvent<FormalFrameworkCategoryUpdated>
    {
        public Guid FormalFrameworkCategoryId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public FormalFrameworkCategoryUpdated(
            Guid formalFrameworkCategoryId,
            string name,
            string previousName)
        {
            Id = formalFrameworkCategoryId;
            Name = name;
            PreviousName = previousName;
        }
    }
}
