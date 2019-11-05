namespace OrganisationRegistry.SqlServer.Infrastructure
{
    using System;

    public class OrganisationRegistryDbInitializer
    {
        private static OrganisationRegistryContext _context;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _context = (OrganisationRegistryContext)serviceProvider.GetService(typeof(OrganisationRegistryContext));
        }
    }
}
