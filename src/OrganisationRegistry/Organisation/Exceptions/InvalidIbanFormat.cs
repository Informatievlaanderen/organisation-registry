namespace OrganisationRegistry.Organisation.Exceptions;

using System;

public class InvalidIbanFormat : DomainException
{
    public InvalidIbanFormat(Exception ex)
        : base("Ongeldige IBAN Code.", ex) { }
}
