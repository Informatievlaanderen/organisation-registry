namespace OrganisationRegistry
{
    public class ExternalKeyNotUniqueWithinTypeException : DomainException
    {
        public ExternalKeyNotUniqueWithinTypeException()
            : base("Externe sleutel is niet uniek binnen type.") {  }
    }
}
