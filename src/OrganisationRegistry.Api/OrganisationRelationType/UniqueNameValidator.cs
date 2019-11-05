namespace OrganisationRegistry.Api.OrganisationRelationType
{
    using System;
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.OrganisationRelationType;

    public class UniqueNameValidator : IUniqueNameValidator<OrganisationRelationType>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name)
        {
            return _context.OrganisationRelationTypeList.Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name)
        {
            return _context.OrganisationRelationTypeList
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
