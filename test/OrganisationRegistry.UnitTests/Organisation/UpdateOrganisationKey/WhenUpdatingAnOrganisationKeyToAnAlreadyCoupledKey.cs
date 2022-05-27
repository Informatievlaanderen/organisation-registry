namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationKey;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.KeyTypes;
using OrganisationRegistry.KeyTypes.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class
    WhenUpdatingAnOrganisationKeyToAnAlreadyCoupledKey : Specification<UpdateOrganisationKeyCommandHandler,
        UpdateOrganisationKey>
{
    private readonly Guid _organisationId;
    private readonly Guid _keyAId;
    private readonly Guid _keyBId;
    private readonly Mock<ISecurityService> _securityServiceMock;
    private readonly Guid _organisationKeyBId;

    public WhenUpdatingAnOrganisationKeyToAnAlreadyCoupledKey(ITestOutputHelper helper) : base(helper)
    {
        _securityServiceMock = new Mock<ISecurityService>();
        _securityServiceMock.Setup(
                service =>
                    service.CanUseKeyType(
                        It.IsAny<IUser>(),
                        It.IsAny<Guid>()))
            .Returns(true);

        _organisationId = Guid.NewGuid();
        _keyAId = Guid.NewGuid();
        _organisationKeyBId = Guid.NewGuid();
        _keyBId = Guid.NewGuid();
    }

    protected override UpdateOrganisationKeyCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationKeyCommandHandler>>().Object,
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
            new KeyTypeCreated(_keyAId, "Sleutel A"), new KeyTypeCreated(_keyBId, "Sleutel B"),
            new OrganisationKeyAdded(_organisationId, Guid.NewGuid(), _keyAId, "Sleutel A", "123123456", null, null)
                { Version = 2 },
            new OrganisationKeyAdded(
                    _organisationId,
                    _organisationKeyBId,
                    _keyBId,
                    "Sleutel B",
                    "123123456",
                    null,
                    null)
                { Version = 3 }
        };

    private UpdateOrganisationKey UpdateOrganisationKeyCommand
        => new(
            _organisationKeyBId,
            new OrganisationId(_organisationId),
            new KeyTypeId(_keyAId),
            "987987654",
            new ValidFrom(null),
            new ValidTo(null));

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(UpdateOrganisationKeyCommand, TestUser.AlgemeenBeheerder).ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events).When(UpdateOrganisationKeyCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<KeyAlreadyCoupledToInThisPeriod>()
            .WithMessage("Deze sleutel is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
