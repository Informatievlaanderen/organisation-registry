namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using OrganisationRegistry.Organisation;

    public class KboOrganisationRetrieverStub : IKboOrganisationRetriever
    {
        private readonly IMagdaOrganisationResponse _response;

        public KboOrganisationRetrieverStub(MockMagdaOrganisationResponse response)
            => _response = response;

        public Task<IMagdaOrganisationResponse> RetrieveOrganisation(ClaimsPrincipal user, KboNumber kboNumber)
            => Task.FromResult(_response);
    }

    public class BankAccountStub : IMagdaBankAccount
    {
        public string Iban { get; set; }
        public string Bic { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class LegalFormStub : IMagdaLegalForm
    {
        public string Code { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddressStub : IMagdaAddress
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
