namespace OrganisationRegistry.MandateRoleType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class MandateRoleTypeCommandHandlers :
    BaseCommandHandler<MandateRoleTypeCommandHandlers>,
    ICommandEnvelopeHandler<CreateMandateRoleType>,
    ICommandEnvelopeHandler<UpdateMandateRoleType>
{
    private readonly IUniqueNameValidator<MandateRoleType> _uniqueNameValidator;

    public MandateRoleTypeCommandHandlers(
        ILogger<MandateRoleTypeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<MandateRoleType> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateMandateRoleType> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
            throw new NameNotUnique();

        var mandateRoleType = new MandateRoleType(envelope.Command.MandateRoleTypeId, envelope.Command.Name);
        Session.Add(mandateRoleType);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<UpdateMandateRoleType> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.MandateRoleTypeId, envelope.Command.Name))
            throw new NameNotUnique();

        var mandateRoleType = Session.Get<MandateRoleType>(envelope.Command.MandateRoleTypeId);
        mandateRoleType.Update(envelope.Command.Name);
        await Session.Commit(envelope.User);
    }
}