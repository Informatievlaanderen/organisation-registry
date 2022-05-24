namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAVlimpersOrganisationAsParentForANonVlimpersOrganisation
    : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly Guid _organisationId;
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly Guid _organisationParentId;

    public WhenAddingAVlimpersOrganisationAsParentForANonVlimpersOrganisation(ITestOutputHelper helper) : base(
        helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _organisationId = Guid.NewGuid();
        _organisationParentId = Guid.NewGuid();
    }

    protected override AddOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            session,
            _dateTimeProviderStub);

    private IEvent[] Events
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
            new OrganisationPlacedUnderVlimpersManagement(_organisationId),
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
                null)
        };

    private AddOrganisationParent AddOrganisationParentCommand
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(_dateTimeProviderStub.Today),
            new ValidTo(_dateTimeProviderStub.Today.AddDays(2)));

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(AddOrganisationParentCommand, UserBuilder.VlimpersBeheerder()).ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task AddsAnOrganisationParent()
    {
        await Given(Events).When(AddOrganisationParentCommand, UserBuilder.VlimpersBeheerder())
            .ThenThrows<VlimpersAndNonVlimpersOrganisationCannotBeInParentalRelationship>();
    }
}
