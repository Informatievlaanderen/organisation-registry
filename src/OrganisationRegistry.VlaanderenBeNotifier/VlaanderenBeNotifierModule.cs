namespace OrganisationRegistry.VlaanderenBeNotifier
{
    using Autofac;
    using Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class VlaanderenBeNotifierModule : Module
    {
        public VlaanderenBeNotifierModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            services.Configure<VlaanderenBeNotifierConfiguration>(
                configuration.GetSection(VlaanderenBeNotifierConfiguration.Section));
        }

        protected override void Load(ContainerBuilder builder)
        {
        }
    }
}
