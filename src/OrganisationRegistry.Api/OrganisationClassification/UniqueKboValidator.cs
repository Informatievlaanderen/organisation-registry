namespace OrganisationRegistry.Api.OrganisationClassification
{
    using System;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Organisation;

    public class UniqueKboValidator : IUniqueKboValidator
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
        private readonly IOrganisationRegistryConfiguration _configuration;

        public UniqueKboValidator(Func<Owned<OrganisationRegistryContext>> contextFactory, IOrganisationRegistryConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
        }

        public bool IsKboNumberTaken(KboNumber kboNumber, DateTime? messageValidFrom, DateTime? messageValidTo)
        {
            var dotFormat = kboNumber.ToDotFormat();
            var digitsOnly = kboNumber.ToDigitsOnly();

            using (var context = _contextFactory().Value)
            {
                return context
                    .OrganisationDetail
                    .Any(item => item.KboNumber == dotFormat || item.KboNumber == digitsOnly);
            }
        }
    }
}
