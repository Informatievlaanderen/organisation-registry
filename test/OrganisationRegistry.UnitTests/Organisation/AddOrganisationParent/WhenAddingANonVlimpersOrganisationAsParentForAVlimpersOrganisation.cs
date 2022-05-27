namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingANonVlimpersOrganisationAsParentForAVlimpersOrganisation
    : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationOrganisationParentId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly Guid _organisationParentId;

    public WhenAddingANonVlimpersOrganisationAsParentForAVlimpersOrganisation(ITestOutputHelper helper) : base(
        helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _organisationId = Guid.NewGuid();
        _organisationOrganisationParentId = Guid.NewGuid();
        _organisationParentId = Guid.NewGuid();
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);
    }

    protected override AddOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            session,
            _dateTimeProviderStub
        );

    protected IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                "OVO000012345",
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
            new OrganisationPlacedUnderVlimpersManagement(_organisationId)
        };

    protected AddOrganisationParent AddOrganisationParentCommand
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesNoEvents()
        => await Given(Events)
            .When(AddOrganisationParentCommand, TestUser.VlimpersBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events)
            .When(AddOrganisationParentCommand, TestUser.VlimpersBeheerder)
            .ThenThrows<VlimpersAndNonVlimpersOrganisationCannotBeInParentalRelationship>();
    }
}
