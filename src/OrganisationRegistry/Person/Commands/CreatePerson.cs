namespace OrganisationRegistry.Person.Commands
{
    using System;

    public class CreatePerson : BaseCommand<PersonId>
    {
        public PersonId PersonId => Id;

        public string FirstName { get; }
        public string Name { get; }
        public Sex? Sex { get; }
        public DateTime? DateOfBirth { get; }

        public CreatePerson(
            PersonId personId,
            string firstName,
            string name,
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
}
