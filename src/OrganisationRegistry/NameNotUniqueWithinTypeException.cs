namespace OrganisationRegistry
{
    public class NameNotUniqueWithinTypeException : DomainException
    {
        public NameNotUniqueWithinTypeException()
            : base("Naam is niet uniek binnen type.") { }
    }
}
