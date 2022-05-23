namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassification
{
    using System;
    using System.Linq;
    using OrganisationRegistry.OrganisationClassification;
    using SqlServer.Infrastructure;

    public class UniqueExternalKeyValidator : IUniqueExternalKeyWithinTypeValidator<OrganisationClassification>
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueExternalKeyValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsExternalKeyTaken(string? externalKey, Guid typeId)
        {
            return !string.IsNullOrEmpty(externalKey) &&
                _context.OrganisationClassificationList
                    .AsQueryable()
                    .Where(item => item.OrganisationClassificationTypeId == typeId)
                    .Any(item => item.ExternalKey == externalKey);
        }

        public bool IsExternalKeyTaken(Guid id, string? externalKey, Guid typeId)
        {
            return !string.IsNullOrEmpty(externalKey) &&
                _context.OrganisationClassificationList
                    .AsQueryable()
                    .Where(item => item.Id != id)
                    .Where(item => item.OrganisationClassificationTypeId == typeId)
                    .Any(item => item.ExternalKey == externalKey);
        }
    }
}
