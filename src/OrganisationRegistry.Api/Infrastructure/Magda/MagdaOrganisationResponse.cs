namespace OrganisationRegistry.Api.Infrastructure.Magda;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using global::Magda.GeefOnderneming;
using Organisation;
using CodeSoortOndernemingType = global::Magda.ZoekOnderneming.CodeSoortOndernemingType;

public class MagdaOrganisationResponse : IMagdaOrganisationResponse
{
    private const string KBODateFormat = "yyyy-MM-dd";

    // https://vlaamseoverheid.atlassian.net/wiki/spaces/MG/pages/500074551/Beschrijving+Antwoord+GeefOnderneming-02.00#BeschrijvingAntwoord(GeefOnderneming-02.00)-Adres
    public const string MaatschappelijkeZetelCode = "001";
    public const string TaalcodeNl = "nl";

    public IMagdaName FormalName { get; }
    public IMagdaName ShortName { get; }


    public DateTime? ValidFrom { get; }

    public string? KboNumber { get; }

    public List<IMagdaBankAccount> BankAccounts { get; }

    public IMagdaLegalForm? LegalForm { get; }

    public IMagdaAddress? Address { get; }

    public IMagdaTermination? Termination { get; }

    public IMagdaLegalEntityType LegalEntityType { get; set; }


    public MagdaOrganisationResponse(Onderneming2_0Type? onderneming, IDateTimeProvider dateTimeProvider)
    {
        FormalName = new Name(onderneming?.Namen?.MaatschappelijkeNamen);

        ShortName = new Name(onderneming?.Namen?.AfgekorteNamen);

        ValidFrom = ParseKboDate(onderneming?.Start?.Datum);

        KboNumber = onderneming?.Ondernemingsnummer;

        BankAccounts = onderneming
            ?.Bankrekeningen
            ?.Select(b => new BankAccount(b))
            .Cast<IMagdaBankAccount>()
            .ToList() ?? new List<IMagdaBankAccount>();

        var legalForm = onderneming
            ?.Rechtsvormen
            ?.FirstOrDefault(
                type =>
                    OverlapsWithToday(type, dateTimeProvider.Today));

        if (legalForm != null)
            LegalForm = new MagdaLegalForm(legalForm);

        Address = MagdaAddress.FromAddressesOrNull(onderneming?.Adressen);

        if (onderneming?.Stopzetting != null)
            Termination = new MagdaTermination(onderneming.Stopzetting);

        LegalEntityType = new MagdaLegalEntityType(onderneming?.SoortOnderneming.Code.Value, onderneming?.SoortOnderneming.Code.Beschrijving);
    }

    private static bool IsValidAddress(AdresOndernemingType a)
        => a.Straat != null && a.Huisnummer != null && a.Gemeente != null && a.Land != null;

    private static bool IsDutchDescriptionPresent(AdresOndernemingType a)
        => a.Descripties?.Any(IsDutch) == true;

    private static bool IsMaatschappelijkeZetel(AdresOndernemingType a)
        => a.Type?.Code?.Value == MaatschappelijkeZetelCode;

    private static bool IsDutch(DescriptieType x)
        => string.Equals(x.Taalcode, TaalcodeNl, StringComparison.InvariantCultureIgnoreCase);

    private static bool OverlapsWithToday(RechtsvormExtentieType type, DateTime today)
        => new Period(
                new ValidFrom(ParseKboDate(type.DatumBegin)),
                new ValidTo(ParseKboDate(type.DatumEinde)))
            .OverlapsWith(today);

    public class Name : IMagdaName
    {
        public string Value { get; }
        public DateTime? ValidFrom { get; }

        public Name(NaamOndernemingType[]? namen)
        {
            var name = FirstValidDutchOrOtherwise(namen);
            Value = name?.Naam.Trim() ?? string.Empty;
            ValidFrom = ParseKboDate(name?.DatumBegin);
        }

        private static NaamOndernemingType? FirstValidDutchOrOtherwise(NaamOndernemingType[]? maybeNames)
        {
            if (maybeNames is not { } names)
                return null;

            var maybeDutchName = FirstValidOrMostRecent(names.Where(IsDutch));

            if (maybeDutchName is { Naam: { } } dutchName)
                return dutchName;

            return FirstValidOrMostRecent(names);
        }

        private static NaamOndernemingType? FirstValidOrMostRecent(IEnumerable<NaamOndernemingType> names)
        {
            var naamOndernemingTypes = names as NaamOndernemingType[] ?? names.ToArray();

            return naamOndernemingTypes
                       .FirstOrDefault(IsValid) ??
                   naamOndernemingTypes
                       .OrderBy(nameType => nameType.DatumEinde)
                       .FirstOrDefault();
        }

        private static bool IsDutch(NaamOndernemingType x)
            => string.Equals(x.Taalcode, TaalcodeNl, StringComparison.InvariantCultureIgnoreCase);

        private static bool IsValid(NaamOndernemingType x)
            => (string.IsNullOrEmpty(x.DatumBegin) ||
                DateTime.ParseExact(x.DatumBegin, KBODateFormat, null) <= DateTime.UtcNow) &&
               (string.IsNullOrEmpty(x.DatumEinde) ||
                DateTime.ParseExact(x.DatumEinde, KBODateFormat, null) >= DateTime.UtcNow);
    }

    public class BankAccount : IMagdaBankAccount
    {
        public string AccountNumber { get; }
        public string Bic { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public BankAccount(BankrekeningType bankrekeningType)
        {
            var possibleAccountNumbers = new[]
            {
                bankrekeningType.IBAN,
                bankrekeningType.Rekeningnummer,
                bankrekeningType.NietSEPAnummer,
            };

            AccountNumber = possibleAccountNumbers.First(x => !string.IsNullOrEmpty(x));

            Bic = bankrekeningType.BIC;
            ValidFrom = ParseKboDate(bankrekeningType.DatumBegin);
            ValidTo = ParseKboDate(bankrekeningType.DatumEinde);
        }
    }

    public class MagdaLegalForm : IMagdaLegalForm
    {
        public string Code { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public MagdaLegalForm(RechtsvormExtentieType rechtsvormExtentieType)
        {
            Code = rechtsvormExtentieType.Code?.Value ?? string.Empty;
            ValidFrom = ParseKboDate(rechtsvormExtentieType.DatumBegin);
            ValidTo = ParseKboDate(rechtsvormExtentieType.DatumEinde);
        }
    }

    public class MagdaAddress : IMagdaAddress
    {
        public string Country { get; }
        public string City { get; }
        public string ZipCode { get; }
        public string Street { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public MagdaAddress(AdresOndernemingType adresOndernemingType)
        {
            Country = adresOndernemingType.Descripties?[0].Adres?.Land?.Naam?.Trim() ?? string.Empty;
            City = adresOndernemingType.Descripties?[0].Adres?.Gemeente?.Naam?.Trim() ?? string.Empty;
            ZipCode = adresOndernemingType.Gemeente?.PostCode?.Trim() ?? string.Empty;

            var streetName = adresOndernemingType
                .Descripties?[0]
                .Adres?
                .Straat?
                .Naam?
                .Trim();

            var houseNumber = adresOndernemingType
                .Huisnummer
                ?.Trim()
                .TrimStart('0');

            var busNumber = string.IsNullOrEmpty(adresOndernemingType.Busnummer)
                ? ""
                : " bus " + adresOndernemingType.Busnummer;

            Street = $"{streetName} {houseNumber}{busNumber}";

            ValidFrom = ParseKboDate(adresOndernemingType.DatumBegin);
            ValidTo = ParseKboDate(adresOndernemingType.DatumEinde);
        }

        public static MagdaAddress? FromAddressesOrNull(AdresOndernemingType[]? adresOndernemingTypes)
        {
            if (adresOndernemingTypes == null || !adresOndernemingTypes.Any())
            {
                return null;
            }

            var validAddresses = adresOndernemingTypes?
                .Where(IsValidAddress)
                .ToList();

            var address = validAddresses?
                       .FirstOrDefault(a => IsDutchDescriptionPresent(a) && IsMaatschappelijkeZetel(a))
                   ?? validAddresses?.SingleOrDefault(IsMaatschappelijkeZetel);

            return address == null
                ? null
                : new MagdaAddress(address);
        }
    }

    private static DateTime? ParseKboDate(string? maybeDate)
        => string.IsNullOrEmpty(maybeDate)
            ? null
            : DateTime.ParseExact(maybeDate, KBODateFormat, null);
}

public class MagdaTermination : IMagdaTermination
{
    public DateTime Date { get; }
    public string Code { get; }
    public string Reason { get; }

    public MagdaTermination(StopzettingExtentieType stopzetting)
    {
        Date = DateTime.ParseExact(stopzetting.Datum, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        Code = stopzetting.Code.Value ?? string.Empty;
        Reason = stopzetting.Code.Beschrijving ?? string.Empty;
    }
}

public class MagdaLegalEntityType : IMagdaLegalEntityType
{
    public string Code { get; }
    public string Description { get; }

    public MagdaLegalEntityType(string code, string description)
    {
        Code = code;
        Description = description;
    }
}
