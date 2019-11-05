namespace OrganisationRegistry.Api.Kbo
{
    using System;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Organisation;

    public class KboLocationRetriever : IKboLocationRetriever
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;

        public KboLocationRetriever(Func<Owned<OrganisationRegistryContext>> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Guid? RetrieveLocation(IMagdaAddress address)
        {
            using(var context = _contextFactory().Value)
                return context.LocationList
                    .FirstOrDefault(l => l.Country == address.Country &&
                                         l.City == address.City &&
                                         l.ZipCode == address.ZipCode &&
                                         l.Street == address.Street)
                    ?.Id;
        }
    }
}
