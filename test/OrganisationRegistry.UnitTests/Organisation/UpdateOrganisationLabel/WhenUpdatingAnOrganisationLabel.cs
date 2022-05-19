namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLabel;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using LabelType;
using LabelType.Events;
using OrganisationRegistry.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class
    WhenUpdatingAnOrganisationLabel : Specification<UpdateOrganisationLabelCommandHandler, UpdateOrganisationLabel>
{
    private Guid _organisationId;
    private Guid _labelTypeId;
    private Guid _organisationLabelId;
    private const string Value = "13135/123lk.,m";
    private DateTime _validTo;
    private DateTime _validFrom;

    public WhenUpdatingAnOrganisationLabel(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationLabelCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationLabelCommandHandler>>().Object,
            Session,
            new OrganisationRegistryConfigurationStub());

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();

        _labelTypeId = Guid.NewGuid();
        _organisationLabelId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);

        return new List<IEvent>
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
            new LabelTypeCreated(_labelTypeId, "Label A"),
            new OrganisationLabelAdded(
                _organisationId,
                _organisationLabelId,
                _labelTypeId,
                "Label A",
                Value,
                _validFrom,
                _validTo)
        };
    }

    protected override UpdateOrganisationLabel When()
        => new(
            _organisationLabelId,
            new OrganisationId(_organisationId),
            new LabelTypeId(_labelTypeId),
            Value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void AnOrganisationLabelUpdatedEventIsPublished()
    {
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationLabelUpdated>>();
    }

    [Fact]
    public void TheEventContainsTheCorrectData()
    {
        var organisationLabelAdded = PublishedEvents.First().UnwrapBody<OrganisationLabelUpdated>();
        organisationLabelAdded.OrganisationId.Should().Be(_organisationId);
        organisationLabelAdded.LabelTypeId.Should().Be(_labelTypeId);
        organisationLabelAdded.Value.Should().Be(Value);
        organisationLabelAdded.ValidFrom.Should().Be(_validFrom);
        organisationLabelAdded.ValidTo.Should().Be(_validTo);
    }
}
