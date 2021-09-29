namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassification
{
    using System;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using OrganisationRegistry.Organisation;
    using SqlServer.Infrastructure;

    public class UniqueKboValidator : IUniqueKboValidator
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;

        public UniqueKboValidator(Func<Owned<OrganisationRegistryContext>> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public bool IsKboNumberTaken(KboNumber kboNumber)
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
