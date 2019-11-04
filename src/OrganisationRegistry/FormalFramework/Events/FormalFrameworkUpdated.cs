namespace OrganisationRegistry.FormalFramework.Events
{
    using System;

    public class FormalFrameworkUpdated : BaseEvent<FormalFrameworkUpdated>
    {
        public Guid FormalFrameworkId => Id;

        public string Name { get; }
        public string PreviousName { get; }

        public string Code { get; }
        public string PreviousCode { get; }

        public Guid FormalFrameworkCategoryId { get; }
        public Guid PreviousFormalFrameworkCategoryId { get; }

        public string FormalFrameworkCategoryName { get; }
        public string PreviousFormalFrameworkCategoryName { get; }

        public FormalFrameworkUpdated(
            Guid formalFrameworkId,
            string name,
            string code,
            Guid formalFrameworkCategoryId,
            string formalFrameworkCategoryName,
            string previousName,
            string previousCode,
            Guid previousFormalFrameworkCategoryId,
            string previousFormalFrameworkCategoryName)
        {
            Id = formalFrameworkId;

            Name = name;
            Code = code;
            FormalFrameworkCategoryId = formalFrameworkCategoryId;
            FormalFrameworkCategoryName = formalFrameworkCategoryName;

            PreviousName = previousName;
            PreviousCode = previousCode;
            PreviousFormalFrameworkCategoryId = previousFormalFrameworkCategoryId;
            PreviousFormalFrameworkCategoryName = previousFormalFrameworkCategoryName;
        }
    }
}
