namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;

public interface IMagdaOrganisationResponse
{
    IMagdaName FormalName { get; }
    IMagdaName ShortName { get; }
    DateTime? ValidFrom { get; }
    List<IMagdaBankAccount> BankAccounts { get; }
    IMagdaLegalForm? LegalForm { get; }
    IMagdaAddress? Address { get; }
    IMagdaTermination? Termination { get; }
    IMagdaLegalEntityType LegalEntityType { get; }
}

public interface IMagdaName
{
    string Value { get; }
    DateTime? ValidFrom { get; }
}

public interface IMagdaBankAccount
{
    string AccountNumber { get; }
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

public interface IMagdaTermination
{
    DateTime Date { get; }
    string Code { get; }
    string Reason { get; }
}

public interface IMagdaLegalEntityType
{
    string Code { get; }
    string Description { get; }
}
