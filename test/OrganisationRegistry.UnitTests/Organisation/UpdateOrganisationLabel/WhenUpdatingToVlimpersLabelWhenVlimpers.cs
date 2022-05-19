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
    WhenUpdatingToVlimpersLabelWhenVlimpers : Specification<UpdateOrganisationLabelCommandHandler,
        UpdateOrganisationLabel>
{
    private Guid _organisationId;
    private Guid _vlimpersLabelTypeId;
    private Guid _organisationLabelId;
    private DateTime _validTo;
    private DateTime _validFrom;
    private Guid _nonVlimpersLabelTypeId;
    private const string NonVlimpersLabelTypeName = "Niet vlimpers";
    private const string OvoNumber = "OVO000012345";
    private const string VlimpersLabelTypeName = "Vlimpers";
    private const string Value = "13135/123lk.,m";

    public WhenUpdatingToVlimpersLabelWhenVlimpers(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationLabelCommandHandler BuildHandler()
    {
        var configuration = new OrganisationRegistryConfigurationStub
        {
            Authorization = new AuthorizationConfigurationStub
            {
                LabelIdsAllowedForVlimpers = new[] { _vlimpersLabelTypeId }
            }
        };

        return new UpdateOrganisationLabelCommandHandler(
            new Mock<ILogger<UpdateOrganisationLabelCommandHandler>>().Object,
            Session,
            configuration);
    }

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();

        _vlimpersLabelTypeId = Guid.NewGuid();
        _nonVlimpersLabelTypeId = Guid.NewGuid();

        _organisationLabelId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);

        return new List<IEvent>
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                OvoNumber,
                "K&G",
                Article.None,
                "Kindjes en gezinnetjes",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new LabelTypeCreated(_vlimpersLabelTypeId, VlimpersLabelTypeName),
            new LabelTypeCreated(_nonVlimpersLabelTypeId, NonVlimpersLabelTypeName),
            new OrganisationPlacedUnderVlimpersManagement(_organisationId),
            new OrganisationLabelAdded(
                _organisationId,
                _organisationLabelId,
                _nonVlimpersLabelTypeId,
                NonVlimpersLabelTypeName,
                Value,
                _validFrom,
                _validTo)
        };
    }

    protected override UpdateOrganisationLabel When()
        => new(
            _organisationLabelId,
            new OrganisationId(_organisationId),
            new LabelTypeId(_vlimpersLabelTypeId),
            Value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void ThrowsAnException()
    {
        PublishedEvents.Single().Should().BeOfType<Envelope<OrganisationLabelUpdated>>();
    }


}
