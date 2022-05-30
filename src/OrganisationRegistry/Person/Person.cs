namespace OrganisationRegistry.Person;

using System;
using Events;
using Infrastructure.Domain;

public class Person : AggregateRoot
{
    private Sex? _sex;
    private DateTime? _dateOfBirth;

    public PersonFirstName FirstName { get; private set; }
    public PersonName Name { get; private set; }

    public string FullName => $"{Name} {FirstName}";

    private Person()
    {
        FirstName = new PersonFirstName(string.Empty);
        Name = new PersonName(string.Empty);
    }

    public Person(
        PersonId id,
        PersonFirstName firstName,
        PersonName name,
        Sex? sex,
        DateTime? dateOfBirth)
    {
        FirstName = new PersonFirstName(string.Empty);
        Name = new PersonName(string.Empty);

        ApplyChange(new PersonCreated(
            id,
            firstName,
            name,
            sex,
            dateOfBirth));
    }

    public void Update(
        PersonFirstName firstName,
        PersonName name,
        Sex? sex,
        DateTime? dateOfBirth)
    {
        ApplyChange(new PersonUpdated(
            Id,
            firstName, name, sex, dateOfBirth,
            FirstName, Name, _sex, _dateOfBirth));
    }

    private void Apply(PersonCreated @event)
    {
        Id = @event.PersonId;
        FirstName = new PersonFirstName(@event.FirstName);
        Name = new PersonName(@event.Name);
        _sex = @event.Sex;
        _dateOfBirth = @event.DateOfBirth;
    }

    private void Apply(PersonUpdated @event)
    {
        FirstName = new PersonFirstName(@event.FirstName);
        Name = new PersonName(@event.Name);
        _sex = @event.Sex;
        _dateOfBirth = @event.DateOfBirth;
    }
}