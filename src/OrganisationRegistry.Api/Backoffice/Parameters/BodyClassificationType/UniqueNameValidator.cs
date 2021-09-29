namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassificationType
{
    using System;
    using System.Linq;
    using OrganisationRegistry.BodyClassificationType;
    using SqlServer.Infrastructure;

    public class UniqueNameValidator : IUniqueNameValidator<BodyClassificationType>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueNameValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsNameTaken(string name)
        {
            return _context.BodyClassificationTypeList.Any(item => item.Name == name);
        }

        public bool IsNameTaken(Guid id, string name)
        {
            return _context.BodyClassificationTypeList
                .AsQueryable()
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
