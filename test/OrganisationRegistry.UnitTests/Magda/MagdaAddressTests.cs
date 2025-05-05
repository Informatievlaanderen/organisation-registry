namespace OrganisationRegistry.UnitTests.Magda;

using System;
using System.IO;
using System.Linq;
using Api.Infrastructure.Magda;
using FluentAssertions;
using global::Magda.GeefOnderneming;
using Newtonsoft.Json;
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
        var json = GetType().GetAssociatedResourceJson("bugfix-or-2650");

        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling       = NullValueHandling.Ignore
        };
        var addresses = JsonConvert.DeserializeObject<AdresOndernemingType[]>(json, settings);

        var magdaAddress = MagdaOrganisationResponse.MagdaAddress.FromAddressesOrNull(addresses);

        magdaAddress.Should().NotBeNull();
        magdaAddress!.Street.Should().Be("Simon Bolivarlaan 17 bus 411");
        magdaAddress!.City.Should().Be("Brussel");
        magdaAddress!.ZipCode.Should().Be("1000");
    }
}
