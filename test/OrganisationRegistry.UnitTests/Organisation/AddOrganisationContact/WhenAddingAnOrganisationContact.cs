namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationContact;

using System;
using System.Collections.Generic;
using System.Linq;
using ContactType;
using ContactType.Events;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationContact
    : Specification<AddOrganisationContactCommandHandler, AddOrganisationContact>
{
    private Guid _organisationId;
    private Guid _contactTypeId;
    private Guid _organisationContactId;
    private DateTime _validTo;
    private DateTime _validFrom;
    private readonly string _contactValue = "info@email.com";

    public WhenAddingAnOrganisationContact(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override IUser User
        => new UserBuilder().Build();


    protected override AddOrganisationContactCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationContactCommandHandler>>().Object,
            Session
        );


    protected override IEnumerable<IEvent> Given()
    {
        var dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

        _contactTypeId = Guid.NewGuid();
        _organisationContactId = Guid.NewGuid();
        _validFrom = dateTimeProviderStub.Today;
        _validTo = dateTimeProviderStub.Today.AddDays(2);
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
            new ContactTypeCreated(_contactTypeId, "Contact type A")
        };
    }

    protected override AddOrganisationContact When()
        => new(
            _organisationContactId,
            new OrganisationId(_organisationId),
            new ContactTypeId(_contactTypeId),
            _contactValue,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void AddsAnOrganisationContact()
    {
        var organisationContactAdded = PublishedEvents.First().UnwrapBody<OrganisationContactAdded>();
        organisationContactAdded.OrganisationId.Should().Be(_organisationId);
        organisationContactAdded.Value.Should().Be(_contactValue);
        organisationContactAdded.ContactTypeId.Should().Be(_contactTypeId);
        organisationContactAdded.ContactTypeName.Should().Be("Contact type A");
        organisationContactAdded.OrganisationContactId.Should().Be(_organisationContactId);
        organisationContactAdded.ValidFrom.Should().Be(_validFrom);
        organisationContactAdded.ValidTo.Should().Be(_validTo);
    }
}
