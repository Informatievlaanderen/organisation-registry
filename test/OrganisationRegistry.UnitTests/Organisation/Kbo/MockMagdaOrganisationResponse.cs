namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using System.Collections.Generic;
    using OrganisationRegistry.Organisation;

    public class MockMagdaOrganisationResponse: IMagdaOrganisationResponse
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public List<IMagdaBankAccount> BankAccounts { get; }
        public List<IMagdaLegalForm> LegalForms { get; }
        public List<IMagdaAddress> Addresses { get; }

        public MockMagdaOrganisationResponse()
        {
            BankAccounts = new List<IMagdaBankAccount>();
            LegalForms = new List<IMagdaLegalForm>();
            Addresses = new List<IMagdaAddress>();
        }
    }
}
