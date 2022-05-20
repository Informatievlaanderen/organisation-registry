namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationLabel;

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

public class WhenAddingAnOrganisationLabel : OldSpecification2<AddOrganisationLabelCommandHandler, AddOrganisationLabel>
{
    private Guid _organisationId;
    private Guid _labelId;
    private Guid _organisationLabelId;
    private const string Value = "12345ABC-@#$";
    private DateTime _validTo;
    private DateTime _validFrom;

    public WhenAddingAnOrganisationLabel(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationLabelCommandHandler BuildHandler()
    {
        var securityServiceMock = new Mock<ISecurityService>();
        securityServiceMock
            .Setup(
                service =>
                    service.CanUseLabelType(
                        It.IsAny<IUser>(),
                        It.IsAny<Guid>()))
            .Returns(true);

        return new AddOrganisationLabelCommandHandler(
            new Mock<ILogger<AddOrganisationLabelCommandHandler>>().Object,
            Session,
            new OrganisationRegistryConfigurationStub());
    }

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _labelId = Guid.NewGuid();
        _organisationLabelId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
        _organisationId = Guid.NewGuid();
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
            new LabelTypeCreated(_labelId, "Label A")
        };
    }

    protected override AddOrganisationLabel When()
        => new(
            _organisationLabelId,
            new OrganisationId(_organisationId),
            new LabelTypeId(_labelId),
            Value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void AnOrganisationLabelAddedEventIsPublished()
    {
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationLabelAdded>>();
    }

    [Fact]
    public void TheEventContainsTheCorrectData()
    {
        var organisationLabelAdded = PublishedEvents.First().UnwrapBody<OrganisationLabelAdded>();
        organisationLabelAdded.OrganisationId.Should().Be(_organisationId);
        organisationLabelAdded.LabelTypeId.Should().Be(_labelId);
        organisationLabelAdded.Value.Should().Be(Value);
        organisationLabelAdded.ValidFrom.Should().Be(_validFrom);
        organisationLabelAdded.ValidTo.Should().Be(_validTo);
    }
}
