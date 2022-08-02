namespace OrganisationRegistry.Function;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class FunctionTypeCommandHandlers :
    BaseCommandHandler<FunctionTypeCommandHandlers>,
    ICommandEnvelopeHandler<CreateFunctionType>,
    ICommandEnvelopeHandler<UpdateFunctionType>
{
    private readonly IUniqueNameValidator<FunctionType> _uniqueNameValidator;

    public FunctionTypeCommandHandlers(
        ILogger<FunctionTypeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<FunctionType> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateFunctionType> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var functionType = new FunctionType(envelope.Command.FunctionTypeId, envelope.Command.Name);

                    session.Add(functionType);
                });

    public async Task Handle(ICommandEnvelope<UpdateFunctionType> envelope)
        => await UpdateHandler<FunctionType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.FunctionTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var functionType = session.Get<FunctionType>(envelope.Command.FunctionTypeId);

                    functionType.Update(envelope.Command.Name);
                });
}
