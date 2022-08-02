namespace OrganisationRegistry.Purpose;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class PurposeCommandHandlers :
    BaseCommandHandler<PurposeCommandHandlers>,
    ICommandEnvelopeHandler<CreatePurpose>,
    ICommandEnvelopeHandler<UpdatePurpose>
{
    private readonly IUniqueNameValidator<Purpose> _uniqueNameValidator;

    public PurposeCommandHandlers(
        ILogger<PurposeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<Purpose> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreatePurpose> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var purpose = new Purpose(envelope.Command.PurposeId, envelope.Command.Name);
                    session.Add(purpose);
                });

    public async Task Handle(ICommandEnvelope<UpdatePurpose> envelope)
        => await UpdateHandler<Purpose>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.PurposeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var purpose = session.Get<Purpose>(envelope.Command.PurposeId);
                    purpose.Update(envelope.Command.Name);
                });
}
