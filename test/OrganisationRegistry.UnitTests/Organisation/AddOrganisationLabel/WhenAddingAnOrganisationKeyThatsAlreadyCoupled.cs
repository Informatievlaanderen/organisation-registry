namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationLabel;

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

public class
    WhenAddingAnOrganisationLabelThatsAlreadyCoupled : Specification<AddOrganisationLabelCommandHandler,
        AddOrganisationLabel>
{
    private readonly Guid _organisationId;
    private readonly Guid _labelId;
    private readonly Guid _organisationLabelId;
    private readonly string _value;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    public WhenAddingAnOrganisationLabelThatsAlreadyCoupled(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _labelId = Guid.NewGuid();
        _organisationLabelId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
        _value = "ABC-12-@#$%";
    }

    protected override AddOrganisationLabelCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationLabelCommandHandler>>().Object,
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
            new LabelTypeCreated(_labelId, "Label A"),
            new OrganisationLabelAdded(
                _organisationId,
                _organisationLabelId,
                _labelId,
                "Label A",
                _value,
                _validFrom,
                _validTo)
        };

    private AddOrganisationLabel AddOrganisationLabelCommand
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationId),
            new LabelTypeId(_labelId),
            _value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events)
            .When(AddOrganisationLabelCommand, UserBuilder.AlgemeenBeheerder())
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events)
            .When(AddOrganisationLabelCommand, UserBuilder.AlgemeenBeheerder())
            .ThenThrows<LabelAlreadyCoupledToInThisPeriod>()
            .WithMessage("Dit label is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
