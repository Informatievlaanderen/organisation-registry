namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassificationType
{
    using System;
    using System.Linq;
    using OrganisationRegistry.OrganisationClassificationType;
    using SqlServer.Infrastructure;

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
                .AsQueryable()
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
