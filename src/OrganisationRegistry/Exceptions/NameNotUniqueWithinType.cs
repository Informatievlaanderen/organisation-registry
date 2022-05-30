namespace OrganisationRegistry.Exceptions;

public class NameNotUniqueWithinType : DomainException
{
    public NameNotUniqueWithinType()
        : base("Naam is niet uniek binnen type.") { }
}