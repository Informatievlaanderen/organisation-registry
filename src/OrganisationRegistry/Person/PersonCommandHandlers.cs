namespace OrganisationRegistry.Person;

using System.Threading.Tasks;
using Commands;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class PersonCommandHandlers :
    BaseCommandHandler<PersonCommandHandlers>,
    ICommandEnvelopeHandler<CreatePerson>,
    ICommandEnvelopeHandler<UpdatePerson>
{
    public PersonCommandHandlers(
        ILogger<PersonCommandHandlers> logger,
        ISession session) : base(logger, session)
    { }

    public async Task Handle(ICommandEnvelope<CreatePerson> envelope)
    {
        var person = new Person(envelope.Command.PersonId, envelope.Command.FirstName, envelope.Command.Name, envelope.Command.Sex, envelope.Command.DateOfBirth);
        Session.Add(person);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<UpdatePerson> envelope)
    {
        var person = Session.Get<Person>(envelope.Command.PersonId);
        person.Update(envelope.Command.FirstName, envelope.Command.Name, envelope.Command.Sex, envelope.Command.DateOfBirth);
        await Session.Commit(envelope.User);
    }
}