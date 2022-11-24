namespace OrganisationRegistry.UnitTests.Magda;

using System;
using System.Threading.Tasks;
using OrganisationRegistry.Api.Infrastructure.Magda;
using FluentAssertions;
using global::Magda.RegistreerInschrijving;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Magda.Common;
using OrganisationRegistry.Magda.Responses;
using OrganisationRegistry.Organisation;
using Tests.Shared.Stubs;
using Xunit;

public class KboOrganisationRetrieverTests
{
    private static KboOrganisationRetriever SetUpKboOrganisationRetriever(string kboNr,
        Envelope<GeefOndernemingResponseBody> magdaResponse)
    {
        var geefOndernemingQueryMock = new Mock<IGeefOndernemingQuery>();
        geefOndernemingQueryMock
            .Setup(query => query.Execute(It.IsAny<IUser>(), kboNr))
            .ReturnsAsync(() => magdaResponse);

        var registreerInschrijvingCommandMock = new Mock<IRegistreerInschrijvingCommand>();
        registreerInschrijvingCommandMock
            .Setup(query => query.Execute(It.IsAny<IUser>(), kboNr))
            .ReturnsAsync(() => new Envelope<RegistreerInschrijvingResponseBody>
            {
                Body = new RegistreerInschrijvingResponseBody
                {
                    RegistreerInschrijvingResponse = new RegistreerInschrijvingResponse
                    {
                        Repliek = new RepliekType
                        {
                            Antwoorden = new AntwoordenType
                            {
                                Antwoord = new AntwoordType(),
                            },
                        },
                    },
                },
            });

        return new KboOrganisationRetriever(
            new DateTimeProviderStub(DateTime.Today),
            geefOndernemingQueryMock.Object,
            registreerInschrijvingCommandMock.Object,
            Mock.Of<ILogger<KboOrganisationRetriever>>());
    }

    [Fact]
    public async Task WhenKboNrIsNotFound()
    {
        var kboNr = "0542399749";
        var magdaResponse = await MagdaJsonLoader.Load(kboNr);
        var sut = SetUpKboOrganisationRetriever(kboNr, magdaResponse);

        var organisationResult = await sut.RetrieveOrganisation(Mock.Of<IUser>(), new KboNumber(kboNr));

        organisationResult.ErrorMessages.Should()
            .BeEquivalentTo("Gevraagde ondernemingsnummer bestaat niet in KBO");
    }

    [Fact]
    public async Task WhenBankAccountIsNonIban()
    {
        var kboNr = "0471693974";
        var magdaResponse = await MagdaJsonLoader.Load(kboNr);
        var sut = SetUpKboOrganisationRetriever(kboNr, magdaResponse);

        var organisationResult = await sut.RetrieveOrganisation(Mock.Of<IUser>(), new KboNumber(kboNr));

        var organisation = organisationResult.Value;

        organisation.Should().NotBeNull();

        var bankAccount = organisation.BankAccounts[0];
        bankAccount.Bic.Should().BeNull();
        bankAccount.AccountNumber.Should().Be("310 0000000 00");
        bankAccount.ValidFrom.Should().Be(new DateTime(2000, 06, 13));
        bankAccount.ValidTo.Should().BeNull();
    }

    [Fact]
    public async Task WhenBankAccountOnlyHasNonSepaNr()
    {
        var kboNr = "0202239951";
        var magdaResponse = await MagdaJsonLoader.Load(kboNr);
        var sut = SetUpKboOrganisationRetriever(kboNr, magdaResponse);

        var organisationResult = await sut.RetrieveOrganisation(Mock.Of<IUser>(), new KboNumber(kboNr));

        var organisation = organisationResult.Value;

        organisation.Should().NotBeNull();

        var bankAccount = organisation.BankAccounts[0];
        bankAccount.Bic.Should().Be("ABOCCNBJ220");
        bankAccount.AccountNumber.Should().Be("CH7897654654564465");
        bankAccount.ValidFrom.Should().Be(new DateTime(2016, 07, 31));
        bankAccount.ValidTo.Should().BeNull();
    }

    [Fact]
    public async Task WithoutShortName()
    {
        var kboNr = "0404055577";
        var magdaResponse = await MagdaJsonLoader.Load(kboNr);
        var sut = SetUpKboOrganisationRetriever(kboNr, magdaResponse);

        var organisationResult = await sut.RetrieveOrganisation(Mock.Of<IUser>(), new KboNumber(kboNr));

        var organisation = organisationResult.Value;

        organisation.Should().NotBeNull();

        organisation.FormalName.Value.Should().Be("Bank J. Van Breda en Co");
        organisation.FormalName.ValidFrom.Should().Be(new DateTime(1930, 02, 21));

        organisation.ShortName.Value.Should().BeEmpty();
        organisation.ShortName.ValidFrom.Should().BeNull();

        organisation.LegalForm!.Code.Should().Be("014");
        organisation.LegalForm.ValidFrom.Should().Be(new DateTime(1998, 04, 08));
        organisation.LegalForm.ValidTo.Should().BeNull();

        var bankAccount = organisation.BankAccounts[0];
        bankAccount.Bic.Should().Be("JVBABE22");
        bankAccount.AccountNumber.Should().Be("BE65645348992796");
        bankAccount.ValidFrom.Should().Be(new DateTime(1930, 12, 01));
        bankAccount.ValidTo.Should().BeNull();

        organisation.Address!.City.Should().Be("Antwerpen");
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

        var organisationResult = await sut.RetrieveOrganisation(Mock.Of<IUser>(), new KboNumber(kboNr));

        var organisation = organisationResult.Value;

        organisation.Should().NotBeNull();

        organisation.LegalForm.Should().BeNull();
    }

    [Fact]
    public async Task TrimsLeadingSpaces()
    {
        var kboNr = "0860325266";
        var magdaResponse = await MagdaJsonLoader.Load(kboNr);
        var sut = SetUpKboOrganisationRetriever(kboNr, magdaResponse);

        var organisationResult = await sut.RetrieveOrganisation(Mock.Of<IUser>(), new KboNumber(kboNr));

        var organisation = organisationResult.Value;

        organisation.Should().NotBeNull();

        organisation.FormalName.Value.Should().Be("FamilyRadio  FM GOUD");
    }
}
