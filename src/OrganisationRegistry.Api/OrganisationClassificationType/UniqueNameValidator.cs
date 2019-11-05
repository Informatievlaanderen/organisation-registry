namespace OrganisationRegistry.Api.OrganisationClassificationType
{
    using System;
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.OrganisationClassificationType;

    public class UniqueNameValidator : IUniqueNameValidator<OrganisationClassificationType>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name)
        {
            return _context.OrganisationClassificationTypeList.Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name)
        {
            return _context.OrganisationClassificationTypeList
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
