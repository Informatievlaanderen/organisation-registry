namespace OrganisationRegistry.Person;

using System.Threading.Tasks;
using Commands;
using Handling;
using Infrastructure.Authorization;
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
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    var person = new Person(envelope.Command.PersonId, envelope.Command.FirstName, envelope.Command.Name, envelope.Command.Sex, envelope.Command.DateOfBirth);
                    session.Add(person);
                });

    public async Task Handle(ICommandEnvelope<UpdatePerson> envelope)
        => await UpdateHandler<Person>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    var person = session.Get<Person>(envelope.Command.PersonId);
                    person.Update(envelope.Command.FirstName, envelope.Command.Name, envelope.Command.Sex, envelope.Command.DateOfBirth);
                });
}
