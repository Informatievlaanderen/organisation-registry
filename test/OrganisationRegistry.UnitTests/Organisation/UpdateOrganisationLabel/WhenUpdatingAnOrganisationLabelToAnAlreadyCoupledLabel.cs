namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLabel;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using LabelType;
using LabelType.Events;
using OrganisationRegistry.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationLabelToAnAlreadyCoupledLabel :
    Specification<UpdateOrganisationLabelCommandHandler, UpdateOrganisationLabel>
{
    private readonly Guid _organisationId;
    private readonly Guid _labelTypeAId;
    private readonly Guid _labelTypeBId;
    private readonly Guid _organisationLabelBId;

    public WhenUpdatingAnOrganisationLabelToAnAlreadyCoupledLabel(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _labelTypeAId = Guid.NewGuid();
        _labelTypeBId = Guid.NewGuid();
        _organisationLabelBId = Guid.NewGuid();
    }

    protected override UpdateOrganisationLabelCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationLabelCommandHandler>>().Object,
            session,
            new OrganisationRegistryConfigurationStub());

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
            new LabelTypeCreated(_labelTypeAId, "Label A"), new LabelTypeCreated(_labelTypeBId, "Label B"),
            new OrganisationLabelAdded(
                _organisationId,
                Guid.NewGuid(),
                _labelTypeAId,
                "Label A",
                "123123456",
                null,
                null) { Version = 2 },
            new OrganisationLabelAdded(
                _organisationId,
                _organisationLabelBId,
                _labelTypeBId,
                "Label B",
                "123123456",
                null,
                null) { Version = 3 },
        };

    private UpdateOrganisationLabel UpdateOrganisationLabelCommand
        => new(
            _organisationLabelBId,
            new OrganisationId(_organisationId),
            new LabelTypeId(_labelTypeAId),
            "987987654",
            new ValidFrom(null),
            new ValidTo(null));

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(UpdateOrganisationLabelCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events).When(UpdateOrganisationLabelCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<LabelAlreadyCoupledToInThisPeriod>()
            .WithMessage("Dit label is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
