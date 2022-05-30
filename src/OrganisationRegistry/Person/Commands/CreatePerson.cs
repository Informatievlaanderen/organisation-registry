namespace OrganisationRegistry.Person.Commands;

using System;

public class CreatePerson : BaseCommand<PersonId>
{
    public PersonId PersonId => Id;

    public PersonFirstName FirstName { get; }
    public PersonName Name { get; }
    public Sex? Sex { get; }
    public DateTime? DateOfBirth { get; }

    public CreatePerson(
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
}