namespace OrganisationRegistry.Person;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class PersonFirstName : StringValueObject<PersonFirstName>
{
    public PersonFirstName([JsonProperty("firstName")] string personFirstName) : base(personFirstName) { }
}
