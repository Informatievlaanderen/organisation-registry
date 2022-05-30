namespace OrganisationRegistry.Person;

using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class PersonName : StringValueObject<PersonName>
{
    public PersonName([JsonProperty("name")] string personName) : base(personName) { }
}