namespace OrganisationRegistry.KboMutations
{
    using Autofac;
    using Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class KboMutationsModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public KboMutationsModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            _configuration = configuration;
            _services = services;

            _services.Configure<KboMutationsConfiguration>(
                _configuration.GetSection(KboMutationsConfiguration.Section));
        }

        protected override void Load(ContainerBuilder builder)
        {
        }
    }
}
