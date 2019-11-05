namespace OrganisationRegistry.Api.BodynClassificationType
{
    using SqlServer.Infrastructure;
    using System;
    using System.Linq;
    using OrganisationRegistry.BodyClassificationType;

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
                .Where(item => item.Id != id)
                .Any(item => item.Name == name);
        }
    }
}
