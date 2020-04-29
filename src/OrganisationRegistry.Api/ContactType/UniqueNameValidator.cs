namespace OrganisationRegistry.Api.ContactType
{
    using System;
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.ContactType;

    public class UniqueNameValidator : IUniqueNameValidator<ContactType>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name)
        {
            return _context.ContactTypeList.Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name)
        {
            return _context.ContactTypeList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
