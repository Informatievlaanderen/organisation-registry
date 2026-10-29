namespace OrganisationRegistry.SqlServer.IntegrationTests.HostedServices;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api.HostedServices;
using AutoFixture;
using Body;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OnProjections;
using Organisation;
using OrganisationRegistry.Organisation;
using RemoveBodyMandate = OrganisationRegistry.Body.RemoveBodyMandate;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.SqlServer.Infrastructure;
using TestBases;
using Tests.Shared.Stubs;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class SyncRemovedItemsServiceTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task OneFailingItem_DoesNotStopOtherItems()
    {
        var contextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase($"sync-removed-items-test-{Guid.NewGuid()}", _ => { })
            .Options;

        var failingFunction = new OrganisationFunctionListItem
        {
            OrganisationFunctionId = _fixture.Create<Guid>(),
            OrganisationId = _fixture.Create<Guid>(),
            FunctionName = _fixture.Create<string>(),
            PersonName = _fixture.Create<string>(),
            ScheduledForRemoval = true,
        };
        var bodyMandate = new BodyMandateListItem
        {
            BodyMandateId = _fixture.Create<Guid>(),
            BodyId = _fixture.Create<Guid>(),
            BodySeatId = _fixture.Create<Guid>(),
            BodySeatName = _fixture.Create<string>(),
            BodySeatNumber = _fixture.Create<string>(),
            DelegatorName = _fixture.Create<string>(),
            ScheduledForRemoval = true,
        };
        var succeedingFunction = new OrganisationFunctionListItem
        {
            OrganisationFunctionId = _fixture.Create<Guid>(),
            OrganisationId = _fixture.Create<Guid>(),
            FunctionName = _fixture.Create<string>(),
            PersonName = _fixture.Create<string>(),
            ScheduledForRemoval = true,
        };

        await using (var setupContext = new OrganisationRegistryContext(contextOptions))
        {
            // order matters: the failing function is returned first
            setupContext.OrganisationFunctionList.Add(failingFunction);
            setupContext.OrganisationFunctionList.Add(succeedingFunction);
            setupContext.BodyMandateList.Add(bodyMandate);
            await setupContext.SaveChangesAsync();
        }

        var commandSenderMock = new Mock<ICommandSender>();
        commandSenderMock
            .Setup(sender => sender.Send(
                It.Is<RemoveOrganisationFunction>(cmd => cmd.OrganisationFunctionId == failingFunction.OrganisationFunctionId),
                It.IsAny<IUser>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var configuration = new TestConfiguration();

        var service = new SyncRemovedItemsService(
            new TestContextFactory(contextOptions),
            commandSenderMock.Object,
            configuration,
            NullLogger<SyncRemovedItemsService>.Instance);

        var cts = new CancellationTokenSource();
        await service.StartAsync(cts.Token);

        // wait for the first pass to complete: all four item types are processed in one pass
        await WaitForPassToComplete(commandSenderMock, expectedSendAttempts: 3, TimeSpan.FromSeconds(10));

        await cts.CancelAsync();
        await service.StopAsync(CancellationToken.None);

        commandSenderMock.Verify(
            sender => sender.Send(
                It.Is<RemoveOrganisationFunction>(cmd => cmd.OrganisationFunctionId == succeedingFunction.OrganisationFunctionId),
                It.IsAny<IUser>()),
            Times.AtLeastOnce);
        commandSenderMock.Verify(
            sender => sender.Send(
                It.Is<RemoveBodyMandate>(cmd => cmd.BodyMandateId == bodyMandate.BodyMandateId),
                It.IsAny<IUser>()),
            Times.AtLeastOnce);
    }

    private static async Task WaitForPassToComplete(Mock<ICommandSender> commandSenderMock, int expectedSendAttempts, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            var attempts = commandSenderMock.Invocations.Count(i => i.Method.Name == nameof(ICommandSender.Send));
            if (attempts >= expectedSendAttempts)
                return;

            await Task.Delay(50);
        }

        commandSenderMock.Invocations.Count(i => i.Method.Name == nameof(ICommandSender.Send))
            .Should().BeGreaterOrEqualTo(expectedSendAttempts, "the first cleanup pass should have attempted all items");
    }

    private class TestConfiguration : IOrganisationRegistryConfiguration
    {
        private readonly OrganisationRegistryConfigurationStub _inner = new();

        public Guid KboKeyTypeId => _inner.KboKeyTypeId;
        public Guid OrafinKeyTypeId => _inner.OrafinKeyTypeId;
        public Guid VlimpersKeyTypeId => _inner.VlimpersKeyTypeId;
        public string OrafinOvoCode => _inner.OrafinOvoCode;
        public Guid FormalNameLabelTypeId => _inner.FormalNameLabelTypeId;
        public Guid FormalShortNameLabelTypeId => _inner.FormalShortNameLabelTypeId;
        public IKboConfiguration Kbo => _inner.Kbo;
        public IAuthorizationConfiguration Authorization => _inner.Authorization;
        public ICachingConfiguration Caching => _inner.Caching;
        public IHostedServicesConfiguration HostedServices { get; } = new TestHostedServicesConfiguration();
    }

    private class TestHostedServicesConfiguration : IHostedServicesConfiguration
    {
        public HostedServiceConfiguration SyncFromKboService => new(3600, false);
        public HostedServiceConfiguration ScheduledCommandsService => new(3600, false);
        public HostedServiceConfiguration SyncRemovedItemsService => new(3600, true);
        public HostedServiceConfiguration ProcessImportedFileService => new(3600, false);
        public HostedServiceConfiguration MEPCalculatorService => new(3600, false);
    }
}
