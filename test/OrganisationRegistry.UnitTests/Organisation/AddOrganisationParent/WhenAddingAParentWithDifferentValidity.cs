namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent;

using System;
using System.Collections.Generic;
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

public class
    WhenAddingAParentWithDifferentValidity
    : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationOrganisationParentId1;
    private readonly Guid _organisationOrganisationParentId2;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private readonly Guid _organisationParentId;

    private readonly string _ovoNumber;
    private readonly string _parentOrganisationName;

    public WhenAddingAParentWithDifferentValidity(ITestOutputHelper helper) : base(helper)
    {
        _organisationOrganisationParentId1 = Guid.NewGuid();
        _organisationOrganisationParentId2 = Guid.NewGuid();
        _validFrom = _dateTimeProviderStub.Today.AddYears(1);
        _validTo = _dateTimeProviderStub.Today.AddYears(1).AddDays(2);
        _organisationId = Guid.NewGuid();
        _organisationParentId = Guid.NewGuid();
        _ovoNumber = "OVO000012345";
        _parentOrganisationName = "Parent Organisation";
    }

    protected override AddOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
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
                _parentOrganisationName,
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
                _organisationOrganisationParentId1,
                _organisationParentId,
                _parentOrganisationName,
                _dateTimeProviderStub.Today,
                _dateTimeProviderStub.Today),
            new ParentAssignedToOrganisation(
                _organisationId,
                _organisationParentId,
                _organisationOrganisationParentId1),
        };

    private AddOrganisationParent AddOrganisationParentCommand
        => new(
            _organisationOrganisationParentId2,
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(AddOrganisationParentCommand, User).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task AddsAnOrganisationParent()
    {
        await Given(Events).When(AddOrganisationParentCommand, User).Then();

        PublishedEvents[0]
            .UnwrapBody<OrganisationParentAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationParentAdded(
                    _organisationId,
                    _organisationOrganisationParentId2,
                    _organisationParentId,
                    _parentOrganisationName,
                    _validFrom,
                    _validTo
                ),
                opt => opt.ExcludeEventProperties());
    }
}
