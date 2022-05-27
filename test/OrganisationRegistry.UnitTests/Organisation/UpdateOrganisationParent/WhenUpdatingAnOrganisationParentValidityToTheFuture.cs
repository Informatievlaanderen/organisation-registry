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

public class WhenUpdatingAnOrganisationParentValidityToTheFuture
    : Specification<UpdateOrganisationParentCommandHandler, UpdateOrganisationParent>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationParentId;
    private readonly Guid _organisationOrganisationParentId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly string _ovoNumber;

    public WhenUpdatingAnOrganisationParentValidityToTheFuture(ITestOutputHelper helper) : base(helper)
    {
        var dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _ovoNumber = "OVO000012345";
        _organisationOrganisationParentId = Guid.NewGuid();
        _validFrom = dateTimeProviderStub.Today;
        _validTo = dateTimeProviderStub.Today.AddDays(2);
        _organisationId = Guid.NewGuid();
        _organisationParentId = Guid.NewGuid();
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
        => new IEvent[] {
            new OrganisationCreated(
                _organisationId,
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
                _organisationParentId,
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
            new OrganisationParentAdded(
                _organisationId,
                _organisationOrganisationParentId,
                _organisationParentId,
                "Ouder en Gezin",
                null,
                null),
            new ParentAssignedToOrganisation(
                _organisationId,
                _organisationParentId,
                _organisationOrganisationParentId)
        };

    private UpdateOrganisationParent UpdateOrganisationParentCommand
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(_validFrom.AddYears(1)),
            new ValidTo(_validTo.AddYears(1)));


    [Fact]
    public async Task PublishesTwoEvents()
    {
        await Given(Events).When(UpdateOrganisationParentCommand, User).ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task UpdatesTheOrganisationBuilding()
    {
        await Given(Events).When(UpdateOrganisationParentCommand, User).Then();
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationParentUpdated>>();

        var organisationParentUpdated = PublishedEvents.First().UnwrapBody<OrganisationParentUpdated>();
        organisationParentUpdated.OrganisationId.Should().Be(_organisationId);
        organisationParentUpdated.ParentOrganisationId.Should().Be(_organisationParentId);
        organisationParentUpdated.ValidFrom.Should().Be(_validFrom.AddYears(1));
        organisationParentUpdated.ValidTo.Should().Be(_validTo.AddYears(1));
    }

    [Fact]
    public async Task ClearsAParent()
    {
        await Given(Events).When(UpdateOrganisationParentCommand, User).Then();
        var parentClearedFromOrganisation = PublishedEvents[1].UnwrapBody<ParentClearedFromOrganisation>();
        parentClearedFromOrganisation.OrganisationId.Should().Be(_organisationId);
        parentClearedFromOrganisation.ParentOrganisationId.Should().Be(_organisationParentId);
    }
}
