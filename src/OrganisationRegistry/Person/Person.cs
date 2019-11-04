namespace OrganisationRegistry.Person
{
    using System;
    using Events;
    using Infrastructure.Domain;

    public class Person : AggregateRoot
    {
        private Sex? _sex;
        private DateTime? _dateOfBirth;

        public string FirstName { get; private set; }
        public string Name { get; private set; }
        public string FullName => $"{Name} {FirstName}";

        private Person() { }

        public Person(PersonId id, string firstName, string name, Sex? sex, DateTime? dateOfBirth)
        {
            var @event = new PersonCreated(id, firstName, name, sex, dateOfBirth);
            ApplyChange(@event);
        }

        public void Update(string firstName, string name, Sex? sex, DateTime? dateOfBirth)
        {
            var @event =
                new PersonUpdated(
                    Id,
                    firstName, name, sex, dateOfBirth,
                    FirstName, Name, _sex, _dateOfBirth);

            ApplyChange(@event);
        }

        private void Apply(PersonCreated @event)
        {
            Id = @event.PersonId;
            FirstName = @event.FirstName;
            Name = @event.Name;
            _sex = @event.Sex;
            _dateOfBirth = @event.DateOfBirth;
        }

        private void Apply(PersonUpdated @event)
        {
            FirstName = @event.FirstName;
            Name = @event.Name;
            _sex = @event.Sex;
            _dateOfBirth = @event.DateOfBirth;
        }
    }
}
