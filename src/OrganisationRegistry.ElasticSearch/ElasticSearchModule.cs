namespace OrganisationRegistry.ElasticSearch;

using Autofac;
using Client;
using Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ElasticSearchModule : Module
{
    public ElasticSearchModule(IConfiguration configuration, IServiceCollection services)
        => services.Configure<ElasticSearchConfiguration>(configuration.GetSection(ElasticSearchConfiguration.Section));

    protected override void Load(ContainerBuilder builder)
        => builder.RegisterType<Elastic>().SingleInstance();
}
