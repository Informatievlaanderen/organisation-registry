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
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

// This test was introduced to fix an issue where the previous organisationId was not correctly set when updating the parent back and forth.
// see the commit of this comment to see how it was fixed.
public class WhenChangingAnInactiveOrganisationParentBackAndForth
    : Specification<UpdateOrganisationParentCommandHandler, UpdateOrganisationParent>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationParentId;
    private readonly Guid _organisationGrandParentId;
    private readonly Guid _organisationOrganisationParentId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    private readonly string _ovoNumber;

    private readonly DateTimeProviderStub _dateTimeProviderStub;

    public WhenChangingAnInactiveOrganisationParentBackAndForth(ITestOutputHelper helper) : base(helper)
    {
        _ovoNumber = "OVO000012345";
        _dateTimeProviderStub =
            new DateTimeProviderStub(new DateTime(2022, 12, 31));
        _organisationOrganisationParentId = Guid.NewGuid();
        _validFrom = _dateTimeProviderStub.Yesterday;
        _validTo = _dateTimeProviderStub.Yesterday;
        _organisationId = Guid.NewGuid();
        _organisationParentId = Guid.NewGuid();
        _organisationGrandParentId = Guid.NewGuid();
    }

    protected override UpdateOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationParentCommandHandler>>().Object,
            session,
            _dateTimeProviderStub);

    private IUser User
        => new UserBuilder()
            .AddOrganisations(_ovoNumber)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                _ovoNumber,
                "K&G",
                Article.None,
                "Kindjes en gezinnetjes",
                new List<Purpose>(),
                false,
                _dateTimeProviderStub.Yesterday,
                _dateTimeProviderStub.Yesterday,
                new ValidFrom(),
                new ValidTo()),
            new OrganisationCreated(
                _organisationParentId,
                "Ouder en Gezin",
                "OVO000012346",
                "O&G",
                Article.None,
                "Moeder",
                new List<Purpose>(),
                false,
                _dateTimeProviderStub.Yesterday,
                _dateTimeProviderStub.Yesterday,
                new ValidFrom(),
                new ValidTo()),
            new OrganisationCreated(
                _organisationGrandParentId,
                "Grootouder en gezin",
                "OVO000012347",
                "K&G",
                Article.None,
                "Oma",
                new List<Purpose>(),
                false,
                _dateTimeProviderStub.Yesterday,
                _dateTimeProviderStub.Yesterday,
                new ValidFrom(),
                new ValidTo()),
            new OrganisationParentAdded(
                _organisationId,
                _organisationOrganisationParentId,
                _organisationParentId,
                "Ouder en Gezin",
                _dateTimeProviderStub.Yesterday,
                _dateTimeProviderStub.Yesterday),
            new OrganisationParentUpdated(
                _organisationId,
                _organisationOrganisationParentId,
                _organisationGrandParentId,
                "",
                _dateTimeProviderStub.Yesterday,
                _dateTimeProviderStub.Yesterday,
                _organisationParentId,
                "",
                null,
                null),
        };

    private UpdateOrganisationParent UpdateOrganisationParentCommand
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

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
        organisationParentUpdated.OrganisationId.Should().Be(_organisationId);
        organisationParentUpdated.PreviousParentOrganisationId.Should().Be(_organisationGrandParentId);
        organisationParentUpdated.ParentOrganisationId.Should().Be(_organisationParentId);
        organisationParentUpdated.ValidFrom.Should().Be(_validFrom);
        organisationParentUpdated.ValidTo.Should().Be(_validTo);
    }
}
