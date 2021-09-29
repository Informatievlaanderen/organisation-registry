namespace OrganisationRegistry.Api.Backoffice.Parameters.FunctionType
{
    using System;
    using System.Linq;
    using Function;
    using SqlServer.Infrastructure;

    public class UniqueNameValidator : IUniqueNameValidator<FunctionType>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name)
        {
            return _context.FunctionTypeList.Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name)
        {
            return _context.FunctionTypeList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
