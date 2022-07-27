namespace OrganisationRegistry.ContactType;

public class ExampleDoesNotMatchRegex : DomainException
{
    public ExampleDoesNotMatchRegex() : base("Voorbeeld komt niet overeen met de opgegeven Regular Expression")
    {
    }
}
