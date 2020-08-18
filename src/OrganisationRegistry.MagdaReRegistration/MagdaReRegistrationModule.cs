namespace OrganisationRegistry.MagdaReRegistration
{
    using Autofac;
    using Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class MagdaReRegistrationModule : Module
    {
        public MagdaReRegistrationModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            services.Configure<MagdaReRegistrationConfiguration>(
                configuration.GetSection(MagdaReRegistrationConfiguration.Section));
        }

        protected override void Load(ContainerBuilder builder)
        {
        }
    }
}
