namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Collections.Generic;

    public interface IMagdaOrganisationResponse
    {
        string Name { get; }
        string ShortName { get; }
        DateTime? ValidFrom { get; }
        List<IMagdaBankAccount> BankAccounts { get; }
        List<IMagdaLegalForm> LegalForms { get; }
        List<IMagdaAddress> Addresses { get; }
    }

    public interface IMagdaBankAccount
    {
        string Iban { get; }
        string Bic { get; }
        DateTime? ValidFrom { get; }
        DateTime? ValidTo { get; }
    }

    public interface IMagdaLegalForm
    {
        string Code { get; }
        DateTime? ValidFrom { get; }
        DateTime? ValidTo { get; }
    }


    public interface IMagdaAddress
    {
        string Country { get; }
        string City { get; }
        string ZipCode { get; }
        string Street { get; }
        DateTime? ValidFrom { get; }
        DateTime? ValidTo { get; }
    }
}
