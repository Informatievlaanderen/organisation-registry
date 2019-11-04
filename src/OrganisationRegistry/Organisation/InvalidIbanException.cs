namespace OrganisationRegistry.Organisation
{
    using System;

    public class InvalidIbanException : DomainException
    {
        public InvalidIbanException(Exception ex)
            : base("Ongeldige IBAN Code.", ex) { }
    }
}
