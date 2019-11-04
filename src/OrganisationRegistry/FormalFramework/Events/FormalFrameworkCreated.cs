namespace OrganisationRegistry.FormalFramework.Events
{
    using System;

    public class FormalFrameworkCreated : BaseEvent<FormalFrameworkCreated>
    {
        public Guid FormalFrameworkId => Id;

        public string Name { get; }
        public string Code { get; }
        public Guid FormalFrameworkCategoryId { get; }
        public string FormalFrameworkCategoryName { get; }

        public FormalFrameworkCreated(
            Guid formalFrameworkId,
            string name,
            string code,
            Guid formalFrameworkCategoryId,
            string formalFrameworkCategoryName)
        {
            Id = formalFrameworkId;
            Name = name;
            Code = code;
            FormalFrameworkCategoryId = formalFrameworkCategoryId;
            FormalFrameworkCategoryName = formalFrameworkCategoryName;
        }
    }
}
