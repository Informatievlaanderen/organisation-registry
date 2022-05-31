namespace OrganisationRegistry.Person.Events;

using System;
using Newtonsoft.Json;

public class PersonUpdated : BaseEvent<PersonUpdated>
{
    public Guid PersonId => Id;

    public string FirstName { get; }
    public string PreviousFirstName { get; }

    public string Name { get; }
    public string PreviousName { get; }

    public Sex? Sex { get; }
    public Sex? PreviousSex { get; }

    public DateTime? DateOfBirth { get; }
    public DateTime? PreviousDateOfBirth { get; }

    public PersonUpdated(
        PersonId personId,
        PersonFirstName firstName,
        PersonName name,
        Sex? sex,
        DateTime? dateOfBirth,
        PersonFirstName previousFirstName,
        PersonName previousName,
        Sex? previousSex,
        DateTime? previousDateOfBirth)
    {
        Id = personId;

        FirstName = firstName;
        Name = name;
        Sex = sex;
        DateOfBirth = dateOfBirth;

        PreviousFirstName = previousFirstName;
        PreviousName = previousName;
        PreviousSex = previousSex;
        PreviousDateOfBirth = previousDateOfBirth;
    }

    [JsonConstructor]
    public PersonUpdated(
        Guid personId,
        string firstName,
        string name,
        Sex? sex,
        DateTime? dateOfBirth,
        string previousFirstName,
        string previousName,
        Sex? previousSex,
        DateTime? previousDateOfBirth)
        : this(
            new PersonId(personId),
            new PersonFirstName(firstName),
            new PersonName(name),
            sex,
            dateOfBirth,
            new PersonFirstName(previousFirstName),
            new PersonName(previousName),
            previousSex,
            previousDateOfBirth)
    {}
}
