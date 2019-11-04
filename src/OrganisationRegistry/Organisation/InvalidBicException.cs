namespace OrganisationRegistry.Organisation
{
    public class InvalidBicException : DomainException
    {
        public InvalidBicException()
            : base("Ongeldige BIC Code.") { }
    }
}
