namespace OrganisationRegistry.Api.Backoffice.Parameters.KeyType
{
    using System;
    using System.Linq;
    using KeyTypes;
    using SqlServer.Infrastructure;

    public class UniqueNameValidator : IUniqueNameValidator<KeyType>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name)
        {
            return _context.KeyTypeList.Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name)
        {
            return _context.KeyTypeList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
