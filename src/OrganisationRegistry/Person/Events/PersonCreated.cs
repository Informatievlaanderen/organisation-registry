namespace OrganisationRegistry.Person.Events
{
    using System;

    public class PersonCreated : BaseEvent<PersonCreated>
    {
        public Guid PersonId => Id;

        public string FirstName { get; }
        public string Name { get; }
        public Sex? Sex { get; }
        public DateTime? DateOfBirth { get; }

        public PersonCreated(
            Guid personId,
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
