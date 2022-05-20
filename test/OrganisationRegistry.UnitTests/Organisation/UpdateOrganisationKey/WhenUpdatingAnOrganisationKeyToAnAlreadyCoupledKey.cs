namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationKey;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.KeyTypes;
using OrganisationRegistry.KeyTypes.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationKeyToAnAlreadyCoupledKey : ExceptionOldSpecification2<UpdateOrganisationKeyCommandHandler, UpdateOrganisationKey>
{
    private OrganisationKeyAdded? _organisationKeyAdded;
    private OrganisationKeyAdded? _anotherOrganisationKeyAdded;
    private Guid _organisationId;
    private Guid _keyAId;
    private Guid _keyBId;

    public WhenUpdatingAnOrganisationKeyToAnAlreadyCoupledKey(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationKeyCommandHandler BuildHandler()
    {
        var securityServiceMock = new Mock<ISecurityService>();
        securityServiceMock.Setup(service =>
                service.CanUseKeyType(
                    It.IsAny<IUser>(),
                    It.IsAny<Guid>()))
            .Returns(true);

        return new UpdateOrganisationKeyCommandHandler(
            new Mock<ILogger<UpdateOrganisationKeyCommandHandler>>().Object,
            Session,
            Mock.Of<IOrganisationRegistryConfiguration>(),
            securityServiceMock.Object);
    }

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();
        _keyAId = Guid.NewGuid();
        _organisationKeyAdded = new OrganisationKeyAdded(_organisationId, Guid.NewGuid(), _keyAId, "Sleutel A", "123123456", null, null) { Version = 2 };
        _keyBId = Guid.NewGuid();
        _anotherOrganisationKeyAdded = new OrganisationKeyAdded(_organisationId, Guid.NewGuid(), _keyBId, "Sleutel B", "123123456", null, null) { Version = 3 };

        return new List<IEvent>
        {
            new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
            new KeyTypeCreated(_keyAId, "Sleutel A"),
            new KeyTypeCreated(_keyBId, "Sleutel B"),
            _organisationKeyAdded,
            _anotherOrganisationKeyAdded
        };
    }

    protected override UpdateOrganisationKey When()
    {
        if (_organisationKeyAdded is not { } || _anotherOrganisationKeyAdded is not { })
            throw new NullReferenceException("You screwed up test setup !");

        return new UpdateOrganisationKey(
            _anotherOrganisationKeyAdded.OrganisationKeyId,
            new OrganisationId(_organisationId),
            new KeyTypeId(_organisationKeyAdded.KeyTypeId),
            "987987654",
            new ValidFrom(null),
            new ValidTo(null));
    }

    protected override int ExpectedNumberOfEvents => 0;

    [Fact]
    public void ThrowsAnException()
    {
        Exception.Should().BeOfType<KeyAlreadyCoupledToInThisPeriod>();
        Exception?.Message.Should().Be("Deze sleutel is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
