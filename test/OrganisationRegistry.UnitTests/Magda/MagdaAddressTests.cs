namespace OrganisationRegistry.UnitTests.Magda;

using System;
using Api.Infrastructure.Magda;
using FluentAssertions;
using global::Magda.GeefOnderneming;
using Xunit;

public class MagdaAddressTests
{
    [Fact]
    public void WithNullAddress_ThenReturnsNull()
    {
        var address = MagdaOrganisationResponse.MagdaAddress.FromAddressesOrNull(null);

        address.Should().BeNull();
    }

    [Fact]
    public void WithNoAddress_ThenReturnsNull()
    {
        var address = MagdaOrganisationResponse.MagdaAddress.FromAddressesOrNull(Array.Empty<AdresOndernemingType>());

        address.Should().BeNull();
    }

    [Fact]
    public void WithFrAndNlAddresses_ThenTakesNlAddress()
    {
        var nlStraatnaam = "nl straatnaam";

        var builder = new AdresOndernemingTypeBuilder();

        var frAdres = builder
            .WithTaalcode("fr")
            .Build();

        var nlAdres = builder
            .WithStraat("Nieuwstraat")
            .WithTaalcode(MagdaOrganisationResponse.TaalcodeNl)
            .Build();

        var magdaAddress = MagdaOrganisationResponse.MagdaAddress.FromAddressesOrNull(new[] { frAdres, nlAdres });

        magdaAddress.Should().NotBeNull();
        magdaAddress!.Street.Should().Be(nlStraatnaam);

    }
}
