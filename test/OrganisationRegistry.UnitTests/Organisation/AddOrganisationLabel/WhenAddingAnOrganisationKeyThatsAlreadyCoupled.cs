namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationLabel;

using System;
using System.Collections.Generic;
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
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class
    WhenAddingAnOrganisationLabelThatsAlreadyCoupled : ExceptionOldSpecification2<AddOrganisationLabelCommandHandler,
        AddOrganisationLabel>
{
    private Guid _organisationId;
    private Guid _labelId;
    private Guid _organisationLabelId;
    private const string Value = "ABC-12-@#$%";
    private DateTime _validTo;
    private DateTime _validFrom;

    public WhenAddingAnOrganisationLabelThatsAlreadyCoupled(ITestOutputHelper helper) : base(helper)
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
        _organisationId = Guid.NewGuid();
        _labelId = Guid.NewGuid();
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
            new LabelTypeCreated(_labelId, "Label A"),
            new OrganisationLabelAdded(
                _organisationId,
                _organisationLabelId,
                _labelId,
                "Label A",
                Value,
                _validFrom,
                _validTo)
        };
    }

    protected override AddOrganisationLabel When()
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationId),
            new LabelTypeId(_labelId),
            Value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsAnException()
    {
        Exception.Should().BeOfType<LabelAlreadyCoupledToInThisPeriod>();
        Exception?.Message.Should().Be("Dit label is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
