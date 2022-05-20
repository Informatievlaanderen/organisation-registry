namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLabel;

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

public class WhenUpdatingAnOrganisationLabelToAnAlreadyCoupledLabel : ExceptionOldSpecification2<
    UpdateOrganisationLabelCommandHandler, UpdateOrganisationLabel>
{
    private Guid _organisationId;
    private Guid _labelTypeAId;
    private Guid _labelTypeBId;
    private Guid _organisationLabelBId;

    public WhenUpdatingAnOrganisationLabelToAnAlreadyCoupledLabel(ITestOutputHelper helper) : base(helper)
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
        _labelTypeAId = Guid.NewGuid();
        _labelTypeBId = Guid.NewGuid();
        _organisationLabelBId = Guid.NewGuid();
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
            new LabelTypeCreated(_labelTypeAId, "Label A"),
            new LabelTypeCreated(_labelTypeBId, "Label B"),
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
                null) { Version = 3 }
        };
    }

    protected override UpdateOrganisationLabel When()
        => new(
            _organisationLabelBId,
            new OrganisationId(_organisationId),
            new LabelTypeId(_labelTypeAId),
            "987987654",
            new ValidFrom(null),
            new ValidTo(null));

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsAnException()
    {
        Exception.Should().BeOfType<LabelAlreadyCoupledToInThisPeriod>();
        Exception?.Message.Should().Be("Dit label is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
