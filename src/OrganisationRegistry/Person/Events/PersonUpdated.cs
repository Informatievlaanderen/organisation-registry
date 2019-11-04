namespace OrganisationRegistry.Person.Events
{
    using System;

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
            Guid personId,
            string firstName,
            string name,
            Sex? sex,
            DateTime? dateOfBirth,
            string previousFirstName,
            string previousName,
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
    }
}
