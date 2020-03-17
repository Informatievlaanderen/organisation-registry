namespace OrganisationRegistry.UnitTests.Magda
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Api.Kbo;
    using FluentAssertions;
    using Moq;
    using OrganisationRegistry.Magda.Common;
    using OrganisationRegistry.Magda.Responses;
    using OrganisationRegistry.Organisation;
    using Xunit;

    public class KboOrganisationRetrieverTests
    {
        private static KboOrganisationRetriever SetUpKboOrganisationRetriever(string kboNr,
            Envelope<GeefOndernemingResponseBody> magdaResponse)
        {
            var geefOndernemingQueryMock = new Mock<IGeefOndernemingQuery>();
            geefOndernemingQueryMock
                .Setup(query => query.Execute(It.IsAny<ClaimsPrincipal>(), kboNr))
                .ReturnsAsync(() => magdaResponse);

            return new KboOrganisationRetriever(
                new DateTimeProviderStub(DateTime.Today),
                geefOndernemingQueryMock.Object);
        }

        [Fact]
        public async Task WhenKboNrIsNotFound()
        {
            var kboNr = "0542399749";
            var magdaResponse = await MagdaJsonLoader.Load(kboNr);
            var sut = SetUpKboOrganisationRetriever(kboNr, magdaResponse);

            var organisation = await sut.RetrieveOrganisation(new ClaimsPrincipal(), new KboNumber(kboNr));

            organisation.Should().BeNull();
        }

        [Fact]
        public async Task WithoutShortName()
        {
            var kboNr = "0404055577";
            var magdaResponse = await MagdaJsonLoader.Load(kboNr);
            var sut = SetUpKboOrganisationRetriever(kboNr, magdaResponse);

            var organisation = await sut.RetrieveOrganisation(new ClaimsPrincipal(), new KboNumber(kboNr));

            organisation.Should().NotBeNull();

            organisation.FormalName.Value.Should().Be("Bank J. Van Breda en Co");
            organisation.FormalName.ValidFrom.Should().Be(new DateTime(1930, 02, 21));

            organisation.ShortName.Value.Should().BeNull();
            organisation.ShortName.ValidFrom.Should().BeNull();

            organisation.LegalForm.Code.Should().Be("014");
            organisation.LegalForm.ValidFrom.Should().Be(new DateTime(1998, 04, 08));
            organisation.LegalForm.ValidTo.Should().BeNull();

            organisation.BankAccounts.Should().BeEmpty();

            organisation.Address.City.Should().Be("Antwerpen");
            organisation.Address.Country.Should().Be("België");
            organisation.Address.Street.Should().Be("Ledeganckkaai 7");
            organisation.Address.ValidFrom.Should().Be(new DateTime(2006, 12, 15));
            organisation.Address.ValidTo.Should().BeNull();
            organisation.Address.ZipCode.Should().Be("2000");

            organisation.ValidFrom.Should().Be(new DateTime(1930, 02, 21));
        }

        [Fact]
        public async Task WithoutLegalForms()
        {
            var kboNr = "0859047440";
            var magdaResponse = await MagdaJsonLoader.Load(kboNr);
            var sut = SetUpKboOrganisationRetriever(kboNr, magdaResponse);

            var organisation = await sut.RetrieveOrganisation(new ClaimsPrincipal(), new KboNumber(kboNr));

            organisation.Should().NotBeNull();

            organisation.LegalForm.Should().BeNull();
        }

        [Fact]
        public async Task TrimsLeadingSpaces()
        {
            var kboNr = "0860325266";
            var magdaResponse = await MagdaJsonLoader.Load(kboNr);
            var sut = SetUpKboOrganisationRetriever(kboNr, magdaResponse);

            var organisation = await sut.RetrieveOrganisation(new ClaimsPrincipal(), new KboNumber(kboNr));

            organisation.Should().NotBeNull();

            organisation.FormalName.Value.Should().Be("FamilyRadio  FM GOUD");
        }
    }
}
