namespace OrganisationRegistry.Api.LabelType
{
    using System;
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.LabelType;

    public class UniqueNameValidator : IUniqueNameValidator<LabelType>
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
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
