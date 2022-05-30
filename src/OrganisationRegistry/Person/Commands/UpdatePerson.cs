namespace OrganisationRegistry.Person.Commands;

using System;

public class UpdatePerson : BaseCommand<PersonId>
{
    public PersonId PersonId => Id;

    public PersonFirstName FirstName { get; }
    public PersonName Name { get; }
    public Sex? Sex { get; }
    public DateTime? DateOfBirth { get; }

    public UpdatePerson(
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