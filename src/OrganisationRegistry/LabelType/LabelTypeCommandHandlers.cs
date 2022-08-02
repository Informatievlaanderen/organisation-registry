namespace OrganisationRegistry.LabelType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class LabelTypeCommandHandlers :
    BaseCommandHandler<LabelTypeCommandHandlers>,
    ICommandEnvelopeHandler<CreateLabelType>,
    ICommandEnvelopeHandler<UpdateLabelType>
{
    private readonly IUniqueNameValidator<LabelType> _uniqueNameValidator;

    public LabelTypeCommandHandlers(
        ILogger<LabelTypeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<LabelType> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateLabelType> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var labelType = new LabelType(envelope.Command.LabelTypeId, envelope.Command.Name);
                    session.Add(labelType);
                });

    public async Task Handle(ICommandEnvelope<UpdateLabelType> envelope)
        => await UpdateHandler<LabelType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.LabelTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var labelType = session.Get<LabelType>(envelope.Command.LabelTypeId);
                    labelType.Update(envelope.Command.Name);
                });
}
