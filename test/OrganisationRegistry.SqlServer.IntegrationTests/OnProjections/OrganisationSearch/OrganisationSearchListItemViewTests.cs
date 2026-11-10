namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationSearch;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Organisation.Events;
using SqlServer.Infrastructure;
using SqlServer.Organisation;
using TestBases;
using Xunit;

/// <summary>
/// OrganisationSearchList is een bewuste kopie van OrganisationList (OR-3785). Deze tests
/// bewaken dat de kopie dezelfde rijen oplevert als het origineel; loopt die uit elkaar, dan
/// vertoont de hoofdpagina ander gedrag dan de rest van de applicatie.
/// </summary>
[Collection(SqlServerTestsCollection.Name)]
public class OrganisationSearchListItemViewTests : ListViewTestBase
{
    [Fact]
    public async Task ProjectsTheSameRowsAsOrganisationList()
    {
        var fixture = new Fixture();

        var parentOrganisationId = fixture.Create<Guid>();
        var childOrganisationId = fixture.Create<Guid>();
        var organisationOrganisationParentId = fixture.Create<Guid>();
        var validFrom = fixture.Create<DateTime>();

        await HandleEvents(
            Created(parentOrganisationId, "Moeder", "OVO000001", "MOEDER", validFrom),
            Created(childOrganisationId, "Dochter", "OVO000002", "DOCHTER", validFrom),
            // OrganisationParentAdded gaat vooraf: andere projecties verwachten de koppeling
            // te kennen voor ze toegekend wordt.
            new OrganisationParentAdded(
                childOrganisationId,
                organisationOrganisationParentId,
                parentOrganisationId,
                "Moeder",
                validFrom,
                null),
            new ParentAssignedToOrganisation(
                childOrganisationId,
                parentOrganisationId,
                organisationOrganisationParentId),
            new OrganisationNameUpdated(childOrganisationId, "Dochter bijgewerkt"),
            new OrganisationShortNameUpdated(childOrganisationId, "DOCHTER-NIEUW"),
            new OrganisationValidityUpdated(childOrganisationId, validFrom, validFrom.AddYears(1)));

        var expected = (await Snapshot(ctx => ctx.OrganisationList)).Select(Project).ToList();
        var actual = (await Snapshot(ctx => ctx.OrganisationSearchList)).Select(Project).ToList();

        expected.Should().NotBeEmpty();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task ProjectsTheSameClassificationValiditiesAsOrganisationList()
    {
        var fixture = new Fixture();

        var organisationId = fixture.Create<Guid>();
        var organisationOrganisationClassificationId = fixture.Create<Guid>();
        var organisationClassificationId = fixture.Create<Guid>();
        var organisationClassificationTypeId = fixture.Create<Guid>();
        var validFrom = fixture.Create<DateTime>();

        await HandleEvents(
            Created(organisationId, "Organisatie", "OVO000003", "ORG", validFrom),
            new OrganisationOrganisationClassificationAdded(
                organisationId,
                organisationOrganisationClassificationId,
                organisationClassificationTypeId,
                "Classificatietype",
                organisationClassificationId,
                "Classificatie",
                validFrom,
                null));

        var expected = (await Snapshot(ctx => ctx.OrganisationClassificationValidities))
            .Select(
                validity => new ValidityRow(
                    validity.OrganisationOrganisationClassificationId,
                    validity.OrganisationClassificationId,
                    validity.OrganisationClassificationTypeId,
                    validity.ValidFrom,
                    validity.ValidTo))
            .ToList();

        var actual = (await Snapshot(ctx => ctx.OrganisationSearchClassificationValidities))
            .Select(
                validity => new ValidityRow(
                    validity.OrganisationOrganisationClassificationId,
                    validity.OrganisationClassificationId,
                    validity.OrganisationClassificationTypeId,
                    validity.ValidFrom,
                    validity.ValidTo))
            .ToList();

        expected.Should().NotBeEmpty();
        actual.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// OrganisationFormalFrameworkAdded is het zwaarste gekopieerde pad: het maakt rijen per
    /// toepassingsgebied aan, voor de organisatie en voor haar moeder.
    /// </summary>
    [Fact]
    public async Task ProjectsTheSameFormalFrameworkRowsAsOrganisationList()
    {
        var fixture = new Fixture();

        var parentOrganisationId = fixture.Create<Guid>();
        var childOrganisationId = fixture.Create<Guid>();
        var organisationFormalFrameworkId = fixture.Create<Guid>();
        var formalFrameworkId = fixture.Create<Guid>();
        var validFrom = fixture.Create<DateTime>();

        await HandleEvents(
            Created(parentOrganisationId, "Moeder", "OVO000004", "MOEDER", validFrom),
            Created(childOrganisationId, "Dochter", "OVO000005", "DOCHTER", validFrom),
            new OrganisationFormalFrameworkAdded(
                childOrganisationId,
                organisationFormalFrameworkId,
                formalFrameworkId,
                "Toepassingsgebied",
                parentOrganisationId,
                "Moeder",
                validFrom,
                null));

        var expected = (await Snapshot(ctx => ctx.OrganisationList)).Select(Project).ToList();
        var actual = (await Snapshot(ctx => ctx.OrganisationSearchList)).Select(Project).ToList();

        expected.Should().HaveCount(4, "moeder en dochter bestaan elk ook als rij voor het toepassingsgebied");
        actual.Should().BeEquivalentTo(expected);
    }

    /// <summary>
    /// Zelfde selectie als de projectielijst in ProjectionsController: wie hierin staat, kan via
    /// de beheer-API herbouwd worden. Zonder herbouw blijven de nieuwe tabellen leeg.
    /// </summary>
    [Fact]
    public void IsListedAsARebuildableProjection()
    {
        var projectionNames = typeof(OrganisationRegistrySqlServerAssemblyTokenClass)
            .Assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false })
            .Where(type => type.GetInterfaces().Any(@interface => @interface == typeof(IProjectionMarker)))
            .Select(type => type.FullName)
            .ToList();

        projectionNames.Should().Contain(typeof(OrganisationSearchListItemView).FullName);
    }

    private async Task<List<T>> Snapshot<T>(Func<Infrastructure.OrganisationRegistryContext, IQueryable<T>> query)
        where T : class
    {
        await using var context = Context;
        return await query(context).ToListAsync();
    }

    /// <summary>De velden die beide read models gemeenschappelijk hebben.</summary>
    private sealed record OrganisationRow(
        Guid OrganisationId,
        string OvoNumber,
        string Name,
        string? ShortName,
        string? ParentOrganisation,
        Guid? ParentOrganisationId,
        string? ParentOrganisationOvoNumber,
        Guid? ParentOrganisationsRelationshipId,
        Guid? FormalFrameworkId,
        DateTime? ValidFrom,
        DateTime? ValidTo);

    private sealed record ValidityRow(
        Guid OrganisationOrganisationClassificationId,
        Guid OrganisationClassificationId,
        Guid OrganisationClassificationTypeId,
        DateTime? ValidFrom,
        DateTime? ValidTo);

    private static OrganisationRow Project(OrganisationListItem item)
        => new(
            item.OrganisationId,
            item.OvoNumber,
            item.Name,
            item.ShortName,
            item.ParentOrganisation,
            item.ParentOrganisationId,
            item.ParentOrganisationOvoNumber,
            item.ParentOrganisationsRelationshipId,
            item.FormalFrameworkId,
            item.ValidFrom,
            item.ValidTo);

    private static OrganisationRow Project(OrganisationSearchListItem item)
        => new(
            item.OrganisationId,
            item.OvoNumber,
            item.Name,
            item.ShortName,
            item.ParentOrganisation,
            item.ParentOrganisationId,
            item.ParentOrganisationOvoNumber,
            item.ParentOrganisationsRelationshipId,
            item.FormalFrameworkId,
            item.ValidFrom,
            item.ValidTo);

    private static OrganisationCreated Created(
        Guid organisationId,
        string name,
        string ovoNumber,
        string shortName,
        DateTime? validFrom)
        => new(
            organisationId,
            name,
            ovoNumber,
            shortName,
            null,
            null,
            new List<Purpose>(),
            false,
            validFrom,
            null,
            null,
            null);
}
