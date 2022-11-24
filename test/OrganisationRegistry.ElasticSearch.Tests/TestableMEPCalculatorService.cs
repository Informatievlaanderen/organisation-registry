namespace OrganisationRegistry.ElasticSearch.Tests;

using System.Threading;
using System.Threading.Tasks;
using Api.HostedServices;
using Client;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Tests.Shared.Stubs;
using SqlServer;

public class TestableMEPCalculatorService : MEPCalculatorService
{
    public TestableMEPCalculatorService(ILogger<TestableMEPCalculatorService> logger, Elastic elastic, IContextFactory contextFactory, IDateTimeProvider clock)
        : base(logger, new OrganisationRegistryConfigurationStub(), elastic, contextFactory, clock)
    {
    }

    public Task TestableProcessBodies(CancellationToken cancellationToken)
        => ProcessBodies(cancellationToken);
}
