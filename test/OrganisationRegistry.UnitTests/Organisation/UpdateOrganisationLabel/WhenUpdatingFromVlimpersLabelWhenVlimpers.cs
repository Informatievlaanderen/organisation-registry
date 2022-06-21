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

public class WhenUpdatingFromVlimpersLabelWhenVlimpers
    : Specification<UpdateOrganisationLabelCommandHandler, UpdateOrganisationLabel>
{
    private readonly Guid _organisationId;
    private readonly Guid _vlimpersLabelTypeId;
    private readonly Guid _organisationLabelId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly Guid _nonVlimpersLabelTypeId;
    private readonly string _ovoNumber;
    private readonly string _value;
    private readonly string _nonVlimpersLabelTypeName;
    private readonly string _vlimpersLabelTypeName;

    public WhenUpdatingFromVlimpersLabelWhenVlimpers(ITestOutputHelper helper) : base(helper)
    {
        _ovoNumber = "OVO000012345";
        _value = "13135/123lk.,m";
        _nonVlimpersLabelTypeName = "Niet vlimpers";
        _vlimpersLabelTypeName = "Vlimpers";
        _organisationId = Guid.NewGuid();

        _vlimpersLabelTypeId = Guid.NewGuid();
        _nonVlimpersLabelTypeId = Guid.NewGuid();

        _organisationLabelId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
    }

    protected override UpdateOrganisationLabelCommandHandler BuildHandler(ISession session)
    {
        var configuration = new OrganisationRegistryConfigurationStub
        {
            Authorization = new AuthorizationConfigurationStub
            {
                LabelIdsAllowedForVlimpers = new[] { _vlimpersLabelTypeId },
            },
        };

        return new UpdateOrganisationLabelCommandHandler(
            new Mock<ILogger<UpdateOrganisationLabelCommandHandler>>().Object,
            session,
            configuration);
    }

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                _ovoNumber,
                "K&G",
                Article.None,
                "Kindjes en gezinnetjes",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new LabelTypeCreated(_vlimpersLabelTypeId, _vlimpersLabelTypeName),
            new LabelTypeCreated(_nonVlimpersLabelTypeId, _nonVlimpersLabelTypeName),
            new OrganisationPlacedUnderVlimpersManagement(_organisationId),
            new OrganisationLabelAdded(
                _organisationId,
                _organisationLabelId,
                _vlimpersLabelTypeId,
                _vlimpersLabelTypeName,
                _value,
                _validFrom,
                _validTo),
        };

    private UpdateOrganisationLabel UpdateOrganisationLabelCommand
        => new(
            _organisationLabelId,
            new OrganisationId(_organisationId),
            new LabelTypeId(_nonVlimpersLabelTypeId),
            _value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));


    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(UpdateOrganisationLabelCommand, TestUser.VlimpersBeheerder).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events).When(UpdateOrganisationLabelCommand, TestUser.VlimpersBeheerder).Then();
        PublishedEvents.Single().Should().BeOfType<Envelope<OrganisationLabelUpdated>>();
    }
}
