namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections;

using System;
using System.Threading.Tasks;
using Api.HostedServices;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Commands;
using TestBases;
using Tests.Shared.Stubs;

public class ScheduledCommandsScenario
{
    public static async Task<(ScheduledCommandsService service, DateTimeProviderStub dateTimeProviderStub, Mock<ICommandSender> commandSender)> Arrange(Action<OrganisationRegistryContext, DateTime> setup)
    {
        var contextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase(
                $"org-es-test-{Guid.NewGuid()}",
                _ => { })
            .Options;

        var testContextFactory = new TestContextFactory(contextOptions);
        var dateTimeProviderStub = new DateTimeProviderStub(new DateTime(2022, 2, 2));

        var loggerMock = new Mock<ILogger<ScheduledCommandsService>>().Object;
        var commandSenderMock = new Mock<ICommandSender>();

        await using var testContext = testContextFactory.Create();
        setup(testContext, dateTimeProviderStub.Today);
        await testContext.SaveChangesAsync();

        return (new ScheduledCommandsService(testContextFactory, dateTimeProviderStub, commandSenderMock.Object, new OrganisationRegistryConfigurationStub(), loggerMock), dateTimeProviderStub, commandSenderMock);
    }
}
