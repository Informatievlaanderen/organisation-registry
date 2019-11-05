namespace OrganisationRegistry.Api.LocationType
{
    using System;
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.LocationType;

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
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
