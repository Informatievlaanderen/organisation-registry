namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationSearch;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.KeyTypes.Events;
using OrganisationRegistry.Organisation.Events;
using SqlServer.Organisation;
using TestBases;
using Xunit;

/// <summary>
/// De codetabel maakt het zoeken op code mogelijk op de hoofdpagina (OR-3785): OVO-nummer,
/// KBO-nummer en organisatiesleutels, elk genormaliseerd.
/// </summary>
[Collection(SqlServerTestsCollection.Name)]
public class OrganisationSearchCodeListTests : ListViewTestBase
{
    [Fact]
    public async Task ProjectsTheOvoNumberOnCreation()
    {
        var organisationId = Guid.NewGuid();

        await HandleEvents(Created(organisationId, "OVO000011"));

        var codes = await CodesOf(organisationId);

        codes.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(
                new
                {
                    Source = OrganisationSearchCodeSource.Ovo,
                    Value = "OVO000011",
                    NormalizedValue = "OVO000011",
                    KeyTypeId = (Guid?)null,
                    ValidFrom = (DateTime?)null,
                    ValidTo = (DateTime?)null,
                });
    }

    [Fact]
    public async Task ProjectsBothCodesWhenCreatedFromKbo()
    {
        var organisationId = Guid.NewGuid();

        await HandleEvents(
            new OrganisationCreatedFromKbo(
                organisationId,
                "0123.456.789",
                "Organisatie",
                "OVO000012",
                "ORG",
                null,
                null,
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null));

        var codes = await CodesOf(organisationId);

        codes.Should().HaveCount(2);
        codes.Should().ContainSingle(code => code.Source == OrganisationSearchCodeSource.Ovo)
            .Which.NormalizedValue.Should().Be("OVO000012");
        codes.Should().ContainSingle(code => code.Source == OrganisationSearchCodeSource.Kbo)
            .Which.Should().BeEquivalentTo(
                new
                {
                    Value = "0123.456.789",
                    NormalizedValue = "0123456789",
                });
    }

    [Fact]
    public async Task ProjectsTheKboNumberWhenCoupledWithKbo()
    {
        var organisationId = Guid.NewGuid();

        await HandleEvents(
            Created(organisationId, "OVO000013"),
            new OrganisationCoupledWithKbo(organisationId, "0987.654.321", "Organisatie", "OVO000013", null));

        var codes = await CodesOf(organisationId);

        codes.Should().ContainSingle(code => code.Source == OrganisationSearchCodeSource.Kbo)
            .Which.NormalizedValue.Should().Be("0987654321");
    }

    [Fact]
    public async Task KeepsOneKboCodeWhenCoupledTwice()
    {
        var organisationId = Guid.NewGuid();

        await HandleEvents(
            Created(organisationId, "OVO000014"),
            new OrganisationCoupledWithKbo(organisationId, "0111.111.111", "Organisatie", "OVO000014", null),
            new OrganisationCoupledWithKbo(organisationId, "0222.222.222", "Organisatie", "OVO000014", null));

        var codes = await CodesOf(organisationId);

        codes.Should().ContainSingle(code => code.Source == OrganisationSearchCodeSource.Kbo)
            .Which.NormalizedValue.Should().Be("0222222222");
    }

    [Fact]
    public async Task ProjectsEveryKeyRegardlessOfItsKeyType()
    {
        var fixture = new Fixture();
        var organisationId = Guid.NewGuid();
        var orafinKeyTypeId = fixture.Create<Guid>();
        var otherKeyTypeId = fixture.Create<Guid>();
        var orafinOrganisationKeyId = fixture.Create<Guid>();
        var validFrom = new DateTime(2020, 1, 1);
        var validTo = new DateTime(2021, 12, 31);

        await HandleEvents(
            Created(organisationId, "OVO000015"),
            new OrganisationKeyAdded(organisationId, orafinOrganisationKeyId, orafinKeyTypeId, "Orafin", "abc-123", validFrom, validTo),
            new OrganisationKeyAdded(organisationId, fixture.Create<Guid>(), otherKeyTypeId, "INR", "INR-9", null, null));

        var codes = await CodesOf(organisationId);

        codes.Where(code => code.Source == OrganisationSearchCodeSource.Key).Should().HaveCount(2);

        codes.Should().ContainSingle(code => code.KeyTypeId == orafinKeyTypeId)
            .Which.Should().BeEquivalentTo(
                new
                {
                    Value = "abc-123",
                    NormalizedValue = "ABC123",
                    OrganisationKeyId = (Guid?)orafinOrganisationKeyId,
                    ValidFrom = (DateTime?)validFrom,
                    ValidTo = (DateTime?)validTo,
                });
    }

    /// <summary>
    /// Een waarde die na normalisatie leeg is, kan nooit gematcht worden; die bewaren we niet.
    /// </summary>
    [Fact]
    public async Task DoesNotProjectValuesWithoutAnyAlphanumerics()
    {
        var fixture = new Fixture();
        var organisationId = Guid.NewGuid();

        await HandleEvents(
            Created(organisationId, "OVO000016"),
            new OrganisationKeyAdded(organisationId, fixture.Create<Guid>(), fixture.Create<Guid>(), "Orafin", "---", null, null));

        var codes = await CodesOf(organisationId);

        codes.Should().NotContain(code => code.Source == OrganisationSearchCodeSource.Key);
    }

    [Fact]
    public async Task UpdatesAKeyCodeWhenTheKeyChanges()
    {
        var fixture = new Fixture();
        var organisationId = Guid.NewGuid();
        var organisationKeyId = fixture.Create<Guid>();
        var keyTypeId = fixture.Create<Guid>();
        var newKeyTypeId = fixture.Create<Guid>();

        await HandleEvents(
            Created(organisationId, "OVO000031"),
            new OrganisationKeyAdded(organisationId, organisationKeyId, keyTypeId, "Orafin", "OUD", null, null),
            new OrganisationKeyUpdated(
                organisationId,
                organisationKeyId,
                newKeyTypeId,
                "Orafin",
                "NIEUW",
                new DateTime(2024, 1, 1),
                new DateTime(2024, 12, 31),
                keyTypeId,
                "Orafin",
                "OUD",
                null,
                null));

        var codes = await CodesOf(organisationId);

        codes.Should().ContainSingle(code => code.Source == OrganisationSearchCodeSource.Key)
            .Which.Should().BeEquivalentTo(
                new
                {
                    Value = "NIEUW",
                    NormalizedValue = "NIEUW",
                    KeyTypeId = (Guid?)newKeyTypeId,
                    ValidFrom = (DateTime?)new DateTime(2024, 1, 1),
                    ValidTo = (DateTime?)new DateTime(2024, 12, 31),
                });
    }

    [Fact]
    public async Task RemovesAKeyCodeWhenTheKeyIsRemoved()
    {
        var fixture = new Fixture();
        var organisationId = Guid.NewGuid();
        var organisationKeyId = fixture.Create<Guid>();

        await HandleEvents(
            Created(organisationId, "OVO000032"),
            new OrganisationKeyAdded(organisationId, organisationKeyId, fixture.Create<Guid>(), "Orafin", "WEG", null, null),
            new OrganisationKeyRemoved(organisationId, organisationKeyId));

        var codes = await CodesOf(organisationId);

        codes.Should().NotContain(code => code.Source == OrganisationSearchCodeSource.Key);
        codes.Should().ContainSingle(code => code.Source == OrganisationSearchCodeSource.Ovo);
    }

    [Fact]
    public async Task RemovesCodesOfARemovedKeyTypeOnly()
    {
        var fixture = new Fixture();
        var organisationId = Guid.NewGuid();
        var removedKeyTypeId = fixture.Create<Guid>();
        var otherKeyTypeId = fixture.Create<Guid>();

        await HandleEvents(
            Created(organisationId, "OVO000033"),
            new KeyTypeCreated(removedKeyTypeId, "Orafin"),
            new KeyTypeCreated(otherKeyTypeId, "INR"),
            new OrganisationKeyAdded(organisationId, fixture.Create<Guid>(), removedKeyTypeId, "Orafin", "VERDWIJNT", null, null),
            new OrganisationKeyAdded(organisationId, fixture.Create<Guid>(), otherKeyTypeId, "INR", "BLIJFT", null, null),
            new KeyTypeRemoved(removedKeyTypeId));

        var codes = await CodesOf(organisationId);

        codes.Should().NotContain(code => code.KeyTypeId == removedKeyTypeId);
        codes.Should().ContainSingle(code => code.KeyTypeId == otherKeyTypeId);
    }

    /// <summary>
    /// Matchen gebeurt op het sleuteltype-id, niet op de naam: een hernoeming raakt dit read
    /// model dus niet.
    /// </summary>
    [Fact]
    public async Task IsUnaffectedByRenamingAKeyType()
    {
        var fixture = new Fixture();
        var organisationId = Guid.NewGuid();
        var keyTypeId = fixture.Create<Guid>();

        await HandleEvents(
            Created(organisationId, "OVO000034"),
            new KeyTypeCreated(keyTypeId, "Orafin"),
            new OrganisationKeyAdded(organisationId, fixture.Create<Guid>(), keyTypeId, "Orafin", "ONGEWIJZIGD", null, null));

        var before = await CodesOf(organisationId);

        await HandleEvents(new KeyTypeUpdated(keyTypeId, "Orafin nieuwe naam", "Orafin"));

        var after = await CodesOf(organisationId);

        after.Should().BeEquivalentTo(before);
    }

    [Fact]
    public async Task RemovesTheKboCodeWhenTheCouplingIsCancelled()
    {
        var organisationId = Guid.NewGuid();

        await HandleEvents(
            Created(organisationId, "OVO000035"),
            new OrganisationCoupledWithKbo(organisationId, "0123.456.789", "Organisatie", "OVO000035", null),
            new OrganisationCouplingWithKboCancelled(
                organisationId,
                "0123.456.789",
                "Organisatie",
                "ORG",
                "Organisatie uit KBO",
                "ORG-KBO",
                "OVO000035",
                null,
                null,
                null,
                new List<Guid>()));

        var codes = await CodesOf(organisationId);

        codes.Should().NotContain(code => code.Source == OrganisationSearchCodeSource.Kbo);
    }

    [Fact]
    public async Task RemovesTheKboCodeWhenTheKboTerminationIsSynced()
    {
        var organisationId = Guid.NewGuid();

        await HandleEvents(
            Created(organisationId, "OVO000036"),
            new OrganisationCoupledWithKbo(organisationId, "0123.456.789", "Organisatie", "OVO000036", null),
            new OrganisationTerminationFoundInKbo(
                organisationId,
                "0123.456.789",
                new DateTime(2024, 6, 30),
                "code",
                "reden"),
            new OrganisationTerminationSyncedWithKbo(
                organisationId,
                "0123.456.789",
                "Organisatie",
                "OVO000036",
                new DateTime(2024, 6, 30),
                null,
                null,
                null,
                new List<Guid>()));

        var codes = await CodesOf(organisationId);

        codes.Should().NotContain(code => code.Source == OrganisationSearchCodeSource.Kbo);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task RemovesTheKboCodeOnTerminationOnlyWhenForced(bool forcedKboTermination, bool kboCodeRemains)
    {
        var organisationId = Guid.NewGuid();

        await HandleEvents(
            Created(organisationId, "OVO000037"),
            new OrganisationCoupledWithKbo(organisationId, "0123.456.789", "Organisatie", "OVO000037", null),
            Terminated(organisationId, "OVO000037", forcedKboTermination, new Dictionary<Guid, DateTime>()));

        var codes = await CodesOf(organisationId);

        codes.Any(code => code.Source == OrganisationSearchCodeSource.Kbo).Should().Be(kboCodeRemains);
    }

    /// <summary>
    /// Bij beëindiging krijgen sleutels een einddatum. Die is in dit read model informatief: de
    /// code blijft doorzoekbaar.
    /// </summary>
    [Fact]
    public async Task RecordsTheTerminationDateOnKeyCodesWithoutRemovingThem()
    {
        var fixture = new Fixture();
        var organisationId = Guid.NewGuid();
        var organisationKeyId = fixture.Create<Guid>();
        var terminationDate = new DateTime(2024, 6, 30);

        await HandleEvents(
            Created(organisationId, "OVO000038"),
            new OrganisationKeyAdded(organisationId, organisationKeyId, fixture.Create<Guid>(), "Orafin", "BEEINDIGD", null, null),
            Terminated(
                organisationId,
                "OVO000038",
                forcedKboTermination: false,
                new Dictionary<Guid, DateTime> { { organisationKeyId, terminationDate } }));

        var codes = await CodesOf(organisationId);

        codes.Should().ContainSingle(code => code.Source == OrganisationSearchCodeSource.Key)
            .Which.ValidTo.Should().Be(terminationDate);
    }

    private static OrganisationTerminatedV2 Terminated(
        Guid organisationId,
        string ovoNumber,
        bool forcedKboTermination,
        Dictionary<Guid, DateTime> keys)
        => new(
            organisationId,
            "Organisatie",
            ovoNumber,
            new DateTime(2024, 6, 30),
            new FieldsToTerminateV2(
                new DateTime(2024, 6, 30),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                new Dictionary<Guid, DateTime>(),
                keys),
            new KboFieldsToTerminateV2(new Dictionary<Guid, DateTime>(), null, null, null),
            forcedKboTermination);

    private async Task<List<OrganisationSearchCodeListItem>> CodesOf(Guid organisationId)
    {
        await using var context = Context;
        return await context.OrganisationSearchCodeList
            .Where(code => code.OrganisationId == organisationId)
            .ToListAsync();
    }

    private static OrganisationCreated Created(Guid organisationId, string ovoNumber)
        => new(
            organisationId,
            "Organisatie",
            ovoNumber,
            "ORG",
            null,
            null,
            new List<Purpose>(),
            false,
            null,
            null,
            null,
            null);
}
