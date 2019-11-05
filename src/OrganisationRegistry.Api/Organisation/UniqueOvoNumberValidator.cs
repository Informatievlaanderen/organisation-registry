namespace OrganisationRegistry.Api.Organisation
{
    using System;
    using System.Linq;
    using SqlServer.Infrastructure;

    public class UniqueOvoNumberValidator : IUniqueOvoNumberValidator
    {
        private readonly OrganisationRegistryContext _context;

        public UniqueOvoNumberValidator(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool IsOvoNumberTaken(string ovoNumber)
        {
            return _context.OrganisationDetail.Any(item => item.OvoNumber == ovoNumber);
        }

        public bool IsOvoNumberTaken(Guid id, string ovoNumber)
        {
            return _context.OrganisationDetail
                .Where(item => item.Id != id)
                .Any(item => item.OvoNumber == ovoNumber);
        }
    }
}
