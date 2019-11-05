namespace OrganisationRegistry.Api.BodyClassification
{
    using SqlServer.Infrastructure;
    using System;
    using System.Linq;
    using OrganisationRegistry.BodyClassification;

    public class UniqueNameValidator : IUniqueNameWithinTypeValidator<BodyClassification>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name, Guid typeId)
        {
            return
                _context.BodyClassificationList
                    .Where(item => item.BodyClassificationTypeId == typeId)
                    .Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name, Guid typeId)
        {
            return
                _context.BodyClassificationList
                    .Where(item => item.Id != id)
                    .Where(item => item.BodyClassificationTypeId == typeId)
                    .Any(item => item.Name == name);
        }
    }
}
