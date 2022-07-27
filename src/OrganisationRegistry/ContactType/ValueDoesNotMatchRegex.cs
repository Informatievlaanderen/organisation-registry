namespace OrganisationRegistry.ContactType;

public class ValueDoesNotMatchRegex : DomainException
{
    public ValueDoesNotMatchRegex(string name, string example): base($"Waarde komt niet overeen met de opgegeven Regular Expression voor {name}. Een correcte waarde zou kunnen zijn: '{example}'")
    {
    }
}
