namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using Purpose;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;
using Xunit.Abstractions;

public class
    WhenTryingToUpdateATerminatedOrgAsBeheerder :
        Specification<UpdateOrganisationCommandHandler, UpdateOrganisationInfo>
{
    private readonly string _ovoNumber;
    private readonly Guid _organisationId;
    private readonly Fixture _fixture;

    public WhenTryingToUpdateATerminatedOrgAsBeheerder(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _ovoNumber = new SequentialOvoNumberGenerator().GenerateNumber();
        _organisationId = Guid.NewGuid();
    }

    protected override UpdateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(DateTime.Today));

    private IUser User
        => new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(_ovoNumber)
            .Build();

    private IEvent[] Events
        => new IEvent[] {
            new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
                .WithId(new OrganisationId(_organisationId))
                .WithOvoNumber(_ovoNumber)
                .WithValidity(null, null)
                .Build(),
            new OrganisationBecameActive(_organisationId), new OrganisationTerminatedV2(
                _organisationId,
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<DateTime>(),
                new FieldsToTerminateV2(
                    null,
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>()),
                new KboFieldsToTerminateV2(
                    new Dictionary<Guid, DateTime>(),
                    new KeyValuePair<Guid, DateTime>?(),
                    new KeyValuePair<Guid, DateTime>?(),
                    new KeyValuePair<Guid, DateTime>?()
                ),
                _fixture.Create<bool>(),
                _fixture.Create<DateTime?>()
            )
        };

    private UpdateOrganisationInfo UpdateOrganisationInfoCommand
        => new(
            new OrganisationId(_organisationId),
            "Test",
            Article.None,
            "testing",
            "shortname",
            new List<PurposeId>(),
            true,
            new ValidFrom(),
            new ValidTo(),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(UpdateOrganisationInfoCommand, User).ThenItPublishesTheCorrectNumberOfEvents(0);
    }
    [Fact]
    public async Task ThrowsOrganisationTerminatedException()
    {
        await Given(Events).When(UpdateOrganisationInfoCommand, User).ThenThrows<OrganisationAlreadyTerminated>();
    }
}
