namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationParent;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenChangingAnOrganisationParent_DoesNotThrowParentClearedAndAssigned
    : Specification<UpdateOrganisationParentCommandHandler, UpdateOrganisationParent>
{
    private readonly Guid _organisationAId;
    private readonly Guid _organisationBId;
    private readonly Guid _organisationCId;
    private readonly Guid _organisationOrganisationParentId;
    private readonly string _ovoNumber;

    public WhenChangingAnOrganisationParent_DoesNotThrowParentClearedAndAssigned(ITestOutputHelper helper) : base(
        helper)
    {
        _ovoNumber = "OVO000012345";
        _organisationOrganisationParentId = Guid.NewGuid();
        _organisationAId = Guid.NewGuid();
        _organisationBId = Guid.NewGuid();
        _organisationCId = Guid.NewGuid();
    }

    protected override UpdateOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationParentCommandHandler>>().Object,
            session,
            new DateTimeProvider());

    private IUser User
        => new UserBuilder()
            .AddOrganisations(_ovoNumber)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreated(
                _organisationAId,
                "Kind en Gezin",
                _ovoNumber,
                "K&G",
                Article.None,
                "Kindjes en gezinnetjes",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new OrganisationCreated(
                _organisationBId,
                "Ouder en Gezin",
                "OVO000012346",
                "O&G",
                Article.None,
                "Moeder",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new OrganisationCreated(
                _organisationCId,
                "Grootouder en gezin",
                "OVO000012347",
                "K&G",
                Article.None,
                "Oma",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new OrganisationParentAdded(
                _organisationAId,
                _organisationOrganisationParentId,
                _organisationBId,
                "Ouder en Gezin",
                null,
                null),
            new ParentAssignedToOrganisation(_organisationAId, _organisationBId, _organisationOrganisationParentId)
        };

    private UpdateOrganisationParent UpdateOrganisationParentCommand
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationAId),
            new OrganisationId(_organisationCId),
            new ValidFrom(),
            new ValidTo());

   [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(UpdateOrganisationParentCommand, User).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task UpdatesTheOrganisationParent()
    {
        await Given(Events).When(UpdateOrganisationParentCommand, User).Then();
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationParentUpdated>>();

        var organisationParentUpdated = PublishedEvents.First().UnwrapBody<OrganisationParentUpdated>();
        organisationParentUpdated.OrganisationId.Should().Be(_organisationAId);
        organisationParentUpdated.PreviousParentOrganisationId.Should().Be(_organisationBId);
        organisationParentUpdated.ParentOrganisationId.Should().Be(_organisationCId);
        organisationParentUpdated.ValidFrom.Should().Be(null);
        organisationParentUpdated.ValidTo.Should().Be(null);
    }
}
