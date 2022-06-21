namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLabel;

using System;
using System.Collections.Generic;
using System.Linq;
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

public class
    WhenUpdatingAnOrganisationLabel : Specification<UpdateOrganisationLabelCommandHandler, UpdateOrganisationLabel>
{
    private readonly Guid _organisationId;
    private readonly Guid _labelTypeId;
    private readonly Guid _organisationLabelId;
    private readonly string _value;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    public WhenUpdatingAnOrganisationLabel(ITestOutputHelper helper) : base(helper)
    {
        _value = "13135/123lk.,m";
        _organisationId = Guid.NewGuid();

        _labelTypeId = Guid.NewGuid();
        _organisationLabelId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
    }

    protected override UpdateOrganisationLabelCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationLabelCommandHandler>>().Object,
            session,
            new OrganisationRegistryConfigurationStub());

    private IEvent[] Events
        => new IEvent[] {
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
            new LabelTypeCreated(_labelTypeId, "Label A"), new OrganisationLabelAdded(
                _organisationId,
                _organisationLabelId,
                _labelTypeId,
                "Label A",
                _value,
                _validFrom,
                _validTo),
        };

    private UpdateOrganisationLabel UpdateOrganisationLabelCommand
        => new(
            _organisationLabelId,
            new OrganisationId(_organisationId),
            new LabelTypeId(_labelTypeId),
            _value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(UpdateOrganisationLabelCommand, TestUser.AlgemeenBeheerder).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task AnOrganisationLabelUpdatedEventIsPublished()
    {
        await Given(Events).When(UpdateOrganisationLabelCommand, TestUser.AlgemeenBeheerder).Then();
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationLabelUpdated>>();
    }

    [Fact]
    public async Task TheEventContainsTheCorrectData()
    {
        await Given(Events).When(UpdateOrganisationLabelCommand, TestUser.AlgemeenBeheerder).Then();
        var organisationLabelAdded = PublishedEvents.First().UnwrapBody<OrganisationLabelUpdated>();
        organisationLabelAdded.OrganisationId.Should().Be(_organisationId);
        organisationLabelAdded.LabelTypeId.Should().Be(_labelTypeId);
        organisationLabelAdded.Value.Should().Be(_value);
        organisationLabelAdded.ValidFrom.Should().Be(_validFrom);
        organisationLabelAdded.ValidTo.Should().Be(_validTo);
    }
}
