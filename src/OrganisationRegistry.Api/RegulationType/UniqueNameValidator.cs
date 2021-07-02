namespace OrganisationRegistry.Api.RegulationType
{
    using System;
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.RegulationType;

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
