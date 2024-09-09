namespace OrganisationRegistry.UnitTests.Organisation.Kbo;

using System;
using System.Collections.Generic;
using Api.Infrastructure.Magda;
using OrganisationRegistry.Organisation;

public class MockMagdaOrganisationResponse : IMagdaOrganisationResponse
{
    public const string MockLegalEntityType = "Natuurlijke Persoon";
    public const string MockLegalEntityTypeCode = "1";
    public IMagdaName FormalName { get; set; } = null!;
    public IMagdaName ShortName { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }
    public List<IMagdaBankAccount> BankAccounts { get; }
    public IMagdaLegalForm? LegalForm { get; set; }
    public IMagdaAddress? Address { get; set; }
    public IMagdaTermination? Termination { get; set; }
    public IMagdaLegalEntityType LegalEntityType { get; }


    public MockMagdaOrganisationResponse()
    {
        LegalEntityType = new MagdaLegalEntityType(MockLegalEntityTypeCode, MockLegalEntityType);
        BankAccounts = new List<IMagdaBankAccount>();
    }
}
