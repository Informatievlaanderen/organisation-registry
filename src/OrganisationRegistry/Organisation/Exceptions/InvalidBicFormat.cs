namespace OrganisationRegistry.Organisation.Exceptions;

using System;

public class InvalidBicFormat : DomainException
{
    public InvalidBicFormat(Exception ex)
        : base("Ongeldige Bic Code.", ex) { }
}
