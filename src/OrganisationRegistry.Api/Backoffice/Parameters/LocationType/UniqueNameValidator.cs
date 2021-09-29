namespace OrganisationRegistry.Api.Backoffice.Parameters.LocationType
{
    using System;
    using System.Linq;
    using OrganisationRegistry.LocationType;
    using SqlServer.Infrastructure;

    public class UniqueNameValidator : IUniqueNameValidator<LocationType>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name)
        {
            return _context.LocationTypeList.Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name)
        {
            return _context.LocationTypeList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
