namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationType
{
    using System;
    using System.Linq;
    using OrganisationRegistry.RegulationType;
    using SqlServer.Infrastructure;

    public class UniqueNameValidator : IUniqueNameValidator<RegulationType>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name)
        {
            return _context.RegulationTypeList.Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name)
        {
            return _context.RegulationTypeList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
