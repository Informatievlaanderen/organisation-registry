namespace OrganisationRegistry.MandateRoleType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
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
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var mandateRoleType = new MandateRoleType(envelope.Command.MandateRoleTypeId, envelope.Command.Name);
                    session.Add(mandateRoleType);
                });

    public async Task Handle(ICommandEnvelope<UpdateMandateRoleType> envelope)
        => await UpdateHandler<MandateRoleType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.MandateRoleTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var mandateRoleType = session.Get<MandateRoleType>(envelope.Command.MandateRoleTypeId);
                    mandateRoleType.Update(envelope.Command.Name);
                });
}
