namespace OrganisationRegistry.Person
{
    using Commands;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;

    public class PersonCommandHandlers :
        BaseCommandHandler<PersonCommandHandlers>,
        ICommandHandler<CreatePerson>,
        ICommandHandler<UpdatePerson>
    {
        public PersonCommandHandlers(
            ILogger<PersonCommandHandlers> logger,
            ISession session) : base(logger, session)
        { }

        public void Handle(CreatePerson message)
        {
            var person = new Person(message.PersonId, message.FirstName, message.Name, message.Sex, message.DateOfBirth);
            Session.Add(person);
            Session.Commit();
        }

        public void Handle(UpdatePerson message)
        {
            var person = Session.Get<Person>(message.PersonId);
            person.Update(message.FirstName, message.Name, message.Sex, message.DateOfBirth);
            Session.Commit();
        }
    }
}
