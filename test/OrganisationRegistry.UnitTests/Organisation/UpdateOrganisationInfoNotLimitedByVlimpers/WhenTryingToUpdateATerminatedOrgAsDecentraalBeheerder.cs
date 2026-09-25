namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfoNotLimitedByVlimpers;

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
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// DecentraalBeheerder holds <see cref="Permission.CanManageOrganisationInfoNotLimitedToVlimpers"/>
/// as a restricted grant for their own organisation, but even so cannot
/// update a terminated organisation.
/// </summary>
public class
    WhenTryingToUpdateATerminatedOrgAsDecentraalBeheerder :
        Specification<UpdateOrganisationNotLimitedToVlimpersCommandHandler, UpdateOrganisationInfoNotLimitedToVlimpers>
{
    private readonly string _ovoNumber;
    private readonly Guid _organisationId;
    private readonly Fixture _fixture;

    public WhenTryingToUpdateATerminatedOrgAsDecentraalBeheerder(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _ovoNumber = new SequentialOvoNumberGenerator().GenerateNumber();
        _organisationId = Guid.NewGuid();
    }

    protected override UpdateOrganisationNotLimitedToVlimpersCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationNotLimitedToVlimpersCommandHandler>>().Object,
            session);

    private IUser User
        => new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(_ovoNumber)
            .WithPermissions(
                RolePermissionMap.For(
                    new[] { Role.DecentraalBeheerder },
                    new OrganisationRegistryConfigurationStub()))
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
            ),
        };

    private UpdateOrganisationInfoNotLimitedToVlimpers UpdateOrganisationInfoNotLimitedToVlimpersCommand
        => new(
            new OrganisationId(_organisationId),
            "testing",
            new List<PurposeId>(),
            true);

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(UpdateOrganisationInfoNotLimitedToVlimpersCommand, User).ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsOrganisationTerminatedException()
    {
        await Given(Events).When(UpdateOrganisationInfoNotLimitedToVlimpersCommand, User).ThenThrows<OrganisationAlreadyTerminated>();
    }
}
