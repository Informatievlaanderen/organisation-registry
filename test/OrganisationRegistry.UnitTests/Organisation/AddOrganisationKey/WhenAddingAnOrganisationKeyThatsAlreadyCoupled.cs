namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationKey;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.KeyTypes;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.KeyTypes.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class
    WhenAddingAnOrganisationKeyThatsAlreadyCoupled : Specification<AddOrganisationKeyCommandHandler, AddOrganisationKey>
{
    private readonly Guid _organisationId;
    private readonly Guid _keyId;
    private readonly Guid _organisationKeyId;
    private readonly string _value;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly Mock<ISecurityService> _securityServiceMock;

    public WhenAddingAnOrganisationKeyThatsAlreadyCoupled(ITestOutputHelper helper) : base(helper)
    {
        _keyId = Guid.NewGuid();
        _organisationKeyId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
        _organisationId = Guid.NewGuid();
        _value = "12345ABC-@#$";

        _securityServiceMock = new Mock<ISecurityService>();
        _securityServiceMock
            .Setup(
                service =>
                    service.CanUseKeyType(
                        It.IsAny<IUser>(),
                        It.IsAny<Guid>()))
            .Returns(true);
    }

    protected override AddOrganisationKeyCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationKeyCommandHandler>>().Object,
            session,
            Mock.Of<IOrganisationRegistryConfiguration>(),
            _securityServiceMock.Object);

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
            new KeyTypeCreated(_keyId, "Key A"),
            new OrganisationKeyAdded(
                _organisationId,
                _organisationKeyId,
                _keyId,
                "Sleutel A",
                _value,
                _validFrom,
                _validTo),
        };


    private AddOrganisationKey AddOrganisationKeyCommand
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationId),
            new KeyTypeId(_keyId),
            _value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events)
            .When(AddOrganisationKeyCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events)
            .When(AddOrganisationKeyCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<KeyAlreadyCoupledToInThisPeriod>()
            .WithMessage("Deze sleutel is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
