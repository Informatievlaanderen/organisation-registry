namespace OrganisationRegistry.Api.Kbo.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Magda.GeefOnderneming;
    using OrganisationRegistry.Organisation;

    public class MagdaOrganisationResponse: IMagdaOrganisationResponse
    {
        private const string KBODateFormat = "yyyy-MM-dd";

        // https://vlaamseoverheid.atlassian.net/wiki/spaces/MG/pages/500074551/Beschrijving+Antwoord+GeefOnderneming-02.00#BeschrijvingAntwoord(GeefOnderneming-02.00)-Adres
        private const string MaatschappelijkeZetelCode = "001";

        public IMagdaName FormalName { get; }
        public IMagdaName ShortName { get; }


        public DateTime? ValidFrom { get; }

        public string KboNumber { get; }

        public List<IMagdaBankAccount> BankAccounts { get; }

        public IMagdaLegalForm LegalForm { get; }

        public IMagdaAddress Address { get; }

        public MagdaOrganisationResponse(Onderneming2_0Type onderneming, IDateTimeProvider dateTimeProvider)
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
                .FirstOrDefault(type =>
                    OverlapsWithToday(type, dateTimeProvider.Today));

            if (legalForm != null)
                LegalForm = new MagdaLegalForm(legalForm);

            var address = onderneming
                ?.Adressen
                ?.Where(a => a.Straat != null && a.Huisnummer != null && a.Gemeente != null && a.Land != null)
                .SingleOrDefault(a => a.Type?.Code?.Value == MaatschappelijkeZetelCode);

            if (address != null)
                Address = new MagdaAddress(address);
        }

        private static bool OverlapsWithToday(RechtsvormExtentieType type, DateTime today)
        {
            return new Period(
                    new ValidFrom(ParseKboDate(type.DatumBegin)),
                    new ValidTo(ParseKboDate(type.DatumEinde)))
                .OverlapsWith(today);
        }

        public class Name : IMagdaName
        {
            public string Value { get; }
            public DateTime? ValidFrom { get; }

            public Name(NaamOndernemingType[] namen)
            {
                var name = FirstValidDutchOrOtherwise(namen);
                Value = name?.Naam;
                ValidFrom = ParseKboDate(name?.DatumBegin);
            }

            private static NaamOndernemingType FirstValidDutchOrOtherwise(NaamOndernemingType[] names)
            {
                if (names == null)
                    return null;

                var dutchName = FirstValidOrMostRecent(names?.Where(IsDutch));

                return dutchName?.Naam != null ?
                    dutchName :
                    FirstValidOrMostRecent(names);
            }

            private static NaamOndernemingType FirstValidOrMostRecent(IEnumerable<NaamOndernemingType> names)
            {
                var naamOndernemingTypes = names as NaamOndernemingType[] ?? names.ToArray();

                return naamOndernemingTypes
                           .FirstOrDefault(IsValid) ??
                       naamOndernemingTypes
                           .OrderBy(nameType => nameType.DatumEinde)
                           .FirstOrDefault();
            }

            private static bool IsDutch(NaamOndernemingType x)
            {
                return x.Taalcode == "nl";
            }

            private static bool IsValid(NaamOndernemingType x)
            {
                return (string.IsNullOrEmpty(x.DatumBegin) || DateTime.ParseExact(x.DatumBegin, KBODateFormat, null) <= DateTime.UtcNow) && (string.IsNullOrEmpty(x.DatumEinde) || DateTime.ParseExact(x.DatumEinde, KBODateFormat, null) >= DateTime.UtcNow);
            }
        }

        public class BankAccount : IMagdaBankAccount
        {
            public string Iban { get; }
            public string Bic { get; }
            public DateTime? ValidFrom { get; }
            public DateTime? ValidTo { get; }

            public BankAccount(BankrekeningType bankrekeningType)
            {
                Iban = bankrekeningType.IBAN;
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
                Code = rechtsvormExtentieType.Code?.Value;
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
                Country = adresOndernemingType.Descripties?[0].Adres?.Land?.Naam?.Trim();
                City = adresOndernemingType.Descripties?[0].Adres?.Gemeente?.Naam?.Trim();
                ZipCode = adresOndernemingType.Gemeente?.PostCode?.Trim();

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
        }

        private static DateTime? ParseKboDate(string d)
        {
            return string.IsNullOrEmpty(d) ? (DateTime?)null : DateTime.ParseExact(d, KBODateFormat, null);
        }
    }
}
