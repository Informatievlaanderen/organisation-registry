namespace OrganisationRegistry.Person.Events;

using System;
using Newtonsoft.Json;

public class PersonCreated : BaseEvent<PersonCreated>
{
    public Guid PersonId => Id;

    public string FirstName { get; }
    public string Name { get; }
    public Sex? Sex { get; }
    public DateTime? DateOfBirth { get; }

    public PersonCreated(
        PersonId personId,
        PersonFirstName firstName,
        PersonName name,
        Sex? sex,
        DateTime? dateOfBirth)
    {
        Id = personId;

        FirstName = firstName;
        Name = name;
        Sex = sex;
        DateOfBirth = dateOfBirth;
    }

    [JsonConstructor]
    public PersonCreated(
        Guid personId,
        string firstName,
        string name,
        Sex? sex,
        DateTime? dateOfBirth)
        : this(
            new PersonId(personId),
            new PersonFirstName(firstName),
            new PersonName(name),
            sex,
            dateOfBirth) { }
}
