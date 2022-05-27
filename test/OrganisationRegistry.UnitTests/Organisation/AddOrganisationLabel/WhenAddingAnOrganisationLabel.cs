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
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationLabel : Specification<AddOrganisationLabelCommandHandler, AddOrganisationLabel>
{
    private readonly Guid _organisationId;
    private readonly Guid _labelId;
    private readonly Guid _organisationLabelId;
    private readonly string _value;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly string _labelName;

    public WhenAddingAnOrganisationLabel(ITestOutputHelper helper) : base(helper)
    {
        _labelId = Guid.NewGuid();
        _organisationLabelId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
        _organisationId = Guid.NewGuid();
        _labelName = "Label A";
        _value = "12345ABC-@#$";
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
            new LabelTypeCreated(_labelId, _labelName)
        };

    private AddOrganisationLabel AddOrganisationLabelCommand
        => new(
            _organisationLabelId,
            new OrganisationId(_organisationId),
            new LabelTypeId(_labelId),
            _value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events)
            .When(AddOrganisationLabelCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task AnOrganisationLabelAddedEventIsPublished()
    {
        await Given(Events).When(AddOrganisationLabelCommand, TestUser.AlgemeenBeheerder).Then();
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationLabelAdded>>();
    }

    [Fact]
    public async Task TheEventContainsTheCorrectData()
    {
        await Given(Events).When(AddOrganisationLabelCommand, TestUser.AlgemeenBeheerder).Then();

        PublishedEvents[0]
            .UnwrapBody<OrganisationLabelAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationLabelAdded(
                    _organisationId,
                    _organisationLabelId,
                    _labelId,
                    _labelName,
                    _value,
                    _validFrom,
                    _validTo),
                opt => opt.ExcludeEventProperties());
    }
}
