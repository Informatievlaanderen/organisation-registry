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

        public string Name { get; }
        public string ShortName { get; }

        public DateTime? ValidFrom { get; }

        public string KboNumber { get; }

        public List<IMagdaBankAccount> BankAccounts { get; }

        public List<IMagdaLegalForm> LegalForms { get; }

        public List<IMagdaAddress> Addresses { get; }

        public MagdaOrganisationResponse(Onderneming2_0Type onderneming)
        {
            var names = onderneming?.Namen?.MaatschappelijkeNamen;
            Name = names
                       ?.Where(IsDutch())
                       .FirstOrDefault(IsValid)
                       ?.Naam ??
                   names?
                       .FirstOrDefault(IsValid)?
                       .Naam;

            var shortNames = onderneming?.Namen?.AfgekorteNamen;
            ShortName = shortNames
                    ?.Where(IsDutch())
                    .FirstOrDefault(IsValid)
                    ?.Naam ??
                shortNames
                    ?.FirstOrDefault(IsValid)?
                    .Naam;

            ValidFrom = ParseKboDate(onderneming?.Start?.Datum);

            KboNumber = onderneming?.Ondernemingsnummer;

            BankAccounts = onderneming
                ?.Bankrekeningen
                ?.Select(b => new BankAccount(b))
                .Cast<IMagdaBankAccount>()
                .ToList() ?? new List<IMagdaBankAccount>();

            LegalForms = onderneming
                ?.Rechtsvormen
                ?.Select(r => new LegalForm(r))
                .Cast<IMagdaLegalForm>()
                .ToList() ?? new List<IMagdaLegalForm>();

            Addresses =
                onderneming
                ?.Adressen
                ?.Where(a => a.Straat != null && a.Huisnummer != null && a.Gemeente != null && a.Land != null)
                .Select(a => new Address(a))
                .Cast<IMagdaAddress>()
                .ToList() ?? new List<IMagdaAddress>();
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

        public class LegalForm : IMagdaLegalForm
        {
            public string Code { get; }
            public DateTime? ValidFrom { get; }
            public DateTime? ValidTo { get; }

            public LegalForm(RechtsvormExtentieType rechtsvormExtentieType)
            {
                Code = rechtsvormExtentieType.Code?.Value;
                ValidFrom = ParseKboDate(rechtsvormExtentieType.DatumBegin);
                ValidTo = ParseKboDate(rechtsvormExtentieType.DatumEinde);
            }
        }

        public class Address : IMagdaAddress
        {
            public string Country { get; }
            public string City { get; }
            public string ZipCode { get; }
            public string Street { get; }
            public DateTime? ValidFrom { get; }
            public DateTime? ValidTo { get; }

            public Address(AdresOndernemingType adresOndernemingType)
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

        private static Func<NaamOndernemingType, bool> IsDutch()
        {
            return x => x.Taalcode == "nl";
        }

        private static bool IsValid(NaamOndernemingType x)
        {
            return (string.IsNullOrEmpty(x.DatumBegin) || DateTime.ParseExact(x.DatumBegin, KBODateFormat, null) <= DateTime.UtcNow) && (string.IsNullOrEmpty(x.DatumEinde) || DateTime.ParseExact(x.DatumEinde, KBODateFormat, null) >= DateTime.UtcNow);
        }

        private static DateTime? ParseKboDate(string d)
        {
            return string.IsNullOrEmpty(d) ? (DateTime?)null : DateTime.ParseExact(d, KBODateFormat, null);
        }
    }
}
