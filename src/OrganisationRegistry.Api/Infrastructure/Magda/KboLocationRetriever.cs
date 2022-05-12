namespace OrganisationRegistry.Api.Infrastructure.Magda
{
    using System;
    using System.Linq;
    using Organisation;
    using SqlServer;

    public class KboLocationRetriever : IKboLocationRetriever
    {
        private readonly IContextFactory _contextFactory;
        public KboLocationRetriever(IContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public Guid? RetrieveLocation(IMagdaAddress address)
        {
            using var context = _contextFactory.Create();
            return context.LocationList
                .FirstOrDefault(l => l.Country == address.Country &&
                                     l.City == address.City &&
                                     l.ZipCode == address.ZipCode &&
                                     l.Street == address.Street)
                ?.Id;
        }
    }
}
