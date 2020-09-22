namespace OrganisationRegistry.UnitTests.Organisation.Kbo
{
    using System;
    using System.Collections.Generic;
    using OrganisationRegistry.Organisation;

    public class MockMagdaOrganisationResponse: IMagdaOrganisationResponse
    {
        public IMagdaName FormalName { get; set; }
        public IMagdaName ShortName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public List<IMagdaBankAccount> BankAccounts { get; }
        public IMagdaLegalForm LegalForm { get; set; }
        public IMagdaAddress Address { get; set; }
        public IMagdaTermination Termination { get; set; }


        public MockMagdaOrganisationResponse()
        {
            BankAccounts = new List<IMagdaBankAccount>();
        }
    }
}
