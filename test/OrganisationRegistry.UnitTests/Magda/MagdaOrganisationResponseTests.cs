namespace OrganisationRegistry.UnitTests.Magda
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Api.Kbo.Responses;
    using FluentAssertions;
    using Newtonsoft.Json;
    using OrganisationRegistry.Magda.Common;
    using OrganisationRegistry.Magda.Responses;
    using Xunit;

    public class MagdaOrganisationResponseTests
    {
        [Fact]
        public async Task WithoutShortName()
        {
            var path = Path.Join("MagdaResponses", "0404055577.json");
            var json = await File.ReadAllTextAsync(path);

            var magdaResponse = JsonConvert.DeserializeObject<Envelope<GeefOndernemingResponseBody>>(json);

            var magdaOrganisationResponse = new MagdaOrganisationResponse(
                magdaResponse.Body.GeefOndernemingResponse.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming,
                new DateTimeProviderStub(DateTime.Today));

            magdaOrganisationResponse.KboNumber.Should().Be("0404055577");

            magdaOrganisationResponse.FormalName.Value.Should().Be("Bank J. Van Breda en Co");
            magdaOrganisationResponse.FormalName.ValidFrom.Should().Be(new DateTime(1930, 02, 21));

            magdaOrganisationResponse.ShortName.Value.Should().BeNull();
            magdaOrganisationResponse.ShortName.ValidFrom.Should().BeNull();

            magdaOrganisationResponse.LegalForm.Code.Should().Be("014");
            magdaOrganisationResponse.LegalForm.ValidFrom.Should().Be(new DateTime(1998, 04, 08));
            magdaOrganisationResponse.LegalForm.ValidTo.Should().BeNull();

            magdaOrganisationResponse.BankAccounts.Should().BeEmpty();

            magdaOrganisationResponse.Address.City.Should().Be("Antwerpen");
            magdaOrganisationResponse.Address.Country.Should().Be("België");
            magdaOrganisationResponse.Address.Street.Should().Be("Ledeganckkaai 7");
            magdaOrganisationResponse.Address.ValidFrom.Should().Be(new DateTime(2006, 12, 15));
            magdaOrganisationResponse.Address.ValidTo.Should().BeNull();
            magdaOrganisationResponse.Address.ZipCode.Should().Be("2000");

            magdaOrganisationResponse.ValidFrom.Should().Be(new DateTime(1930, 02, 21));
        }
    }
}
