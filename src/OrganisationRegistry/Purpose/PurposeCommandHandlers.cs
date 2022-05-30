namespace OrganisationRegistry.Purpose;

using System.Threading.Tasks;
using Commands;
using Exceptions;
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
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
            throw new NameNotUnique();

        var purpose = new Purpose(envelope.Command.PurposeId, envelope.Command.Name);
        Session.Add(purpose);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<UpdatePurpose> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.PurposeId, envelope.Command.Name))
            throw new NameNotUnique();

        var purpose = Session.Get<Purpose>(envelope.Command.PurposeId);
        purpose.Update(envelope.Command.Name);
        await Session.Commit(envelope.User);
    }
}