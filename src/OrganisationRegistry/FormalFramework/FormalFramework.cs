namespace OrganisationRegistry.FormalFramework
{
    using System;
    using Events;
    using FormalFrameworkCategory;
    using Infrastructure.Domain;

    public class FormalFramework : AggregateRoot
    {
        public string Name { get; private set; }
        private string _code;
        private Guid _formalFrameworkCategoryId;
        private string _formalFrameworkCategoryName;

        public FormalFramework() { }

        public FormalFramework(
            FormalFrameworkId id,
            string name,
            string code,
            FormalFrameworkCategory formalFrameworkCategory)
        {
            ApplyChange(
                new FormalFrameworkCreated(
                    id,
                    name,
                    code,
                    formalFrameworkCategory.Id,
                    formalFrameworkCategory.Name));
        }

        public void Update(string name, string code, FormalFrameworkCategory formalFrameworkCategory)
        {
            ApplyChange(
                new FormalFrameworkUpdated(
                    Id,
                    name, code, formalFrameworkCategory.Id, formalFrameworkCategory.Name,
                    Name, _code, _formalFrameworkCategoryId, _formalFrameworkCategoryName));
        }

        private void Apply(FormalFrameworkCreated @event)
        {
            Id = @event.FormalFrameworkId;
            Name = @event.Name;
            _code = @event.Code;
            _formalFrameworkCategoryId = @event.FormalFrameworkCategoryId;
            _formalFrameworkCategoryName = @event.FormalFrameworkCategoryName;
        }

        private void Apply(FormalFrameworkUpdated @event)
        {
            Name = @event.Name;
            _code = @event.Code;
            _formalFrameworkCategoryId = @event.FormalFrameworkCategoryId;
            _formalFrameworkCategoryName = @event.FormalFrameworkCategoryName;
        }
    }
}
