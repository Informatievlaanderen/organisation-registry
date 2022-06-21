namespace OrganisationRegistry.KboMutations.UnitTests;

using System;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class MutationsLineTests
{
    [Fact]
    public void MutationsLinesShouldBeEqual()
    {
        var fixture = new Fixture();

        var datumModificatie = fixture.Create<DateTime>();
        var ondernemingsnummer = fixture.Create<string>();
        var maatschappelijkeNaam = fixture.Create<string>();
        var mutationsLine = new MutationsLine
        {
            DatumModificatie = datumModificatie,
            StatusCode = fixture.Create<string>(),
            Ondernemingsnummer = ondernemingsnummer,
            MaatschappelijkeNaam = maatschappelijkeNaam,
            StopzettingsDatum = fixture.Create<DateTime>(),
            StopzettingsCode = fixture.Create<string>(),
            StopzettingsReden = fixture.Create<string>(),
        };

        var otherMutationsLine = new MutationsLine
        {
            DatumModificatie = datumModificatie,
            StatusCode = fixture.Create<string>(),
            Ondernemingsnummer = ondernemingsnummer,
            MaatschappelijkeNaam = maatschappelijkeNaam,
            StopzettingsDatum = fixture.Create<DateTime>(),
            StopzettingsCode = fixture.Create<string>(),
            StopzettingsReden = fixture.Create<string>(),
        };

        mutationsLine.Should().Be(otherMutationsLine);
    }

    [Fact]
    public void MutationsLinesShouldNotBeEqual()
    {
        var fixture = new Fixture();

        var mutationsLine = new MutationsLine
        {
            DatumModificatie = fixture.Create<DateTime>(),
            StatusCode = fixture.Create<string>(),
            Ondernemingsnummer = fixture.Create<string>(),
            MaatschappelijkeNaam = fixture.Create<string>(),
            StopzettingsDatum = fixture.Create<DateTime>(),
            StopzettingsCode = fixture.Create<string>(),
            StopzettingsReden = fixture.Create<string>(),
        };

        var otherMutationsLine = new MutationsLine
        {
            DatumModificatie = fixture.Create<DateTime>(),
            StatusCode = fixture.Create<string>(),
            Ondernemingsnummer = fixture.Create<string>(),
            MaatschappelijkeNaam = fixture.Create<string>(),
            StopzettingsDatum = fixture.Create<DateTime>(),
            StopzettingsCode = fixture.Create<string>(),
            StopzettingsReden = fixture.Create<string>(),
        };

        mutationsLine.Should().NotBe(otherMutationsLine);
    }
}
