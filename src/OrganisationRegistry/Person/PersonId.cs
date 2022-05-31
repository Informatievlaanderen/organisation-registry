namespace OrganisationRegistry.Person;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class PersonId : GuidValueObject<PersonId>
{
    public PersonId([JsonProperty("id")] Guid personId) : base(personId) { }
}
