namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Organisation;

    public class KboOrganisationRetrieverStub : IKboOrganisationRetriever
    {
        private readonly IMagdaOrganisationResponse _response;

        public KboOrganisationRetrieverStub(MockMagdaOrganisationResponse response)
            => _response = response;

        public Task<Result<IMagdaOrganisationResponse>> RetrieveOrganisation(IUser user, KboNumber kboNumber)
            => Task.FromResult(Result<IMagdaOrganisationResponse>.Success(_response));
    }

    public class NameStub : IMagdaName
    {
        public NameStub(string value, DateTime? validFrom)
        {
            Value = value;
            ValidFrom = validFrom;
        }

        public string Value { get; set; }
        public DateTime? ValidFrom { get; set; }
    }


    public class BankAccountStub : IMagdaBankAccount
    {
        public string AccountNumber { get; set; } = null!;
        public string Bic { get; set; } = null!;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class LegalFormStub : IMagdaLegalForm
    {
        public string Code { get; set; } = null!;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddressStub : IMagdaAddress
    {
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string Street { get; set; } = null!;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
