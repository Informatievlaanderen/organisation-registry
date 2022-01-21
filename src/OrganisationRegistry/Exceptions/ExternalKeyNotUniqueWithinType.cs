namespace OrganisationRegistry.Exceptions
{
    public class ExternalKeyNotUniqueWithinType : DomainException
    {
        public ExternalKeyNotUniqueWithinType()
            : base("Externe sleutel is niet uniek binnen type.") {  }
    }
}
