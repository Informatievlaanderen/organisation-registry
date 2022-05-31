namespace OrganisationRegistry.Function;

using System.Threading.Tasks;
using Commands;
using Exceptions;
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
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
            throw new NameNotUnique();

        var functionType = new FunctionType(envelope.Command.FunctionTypeId, envelope.Command.Name);

        Session.Add(functionType);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<UpdateFunctionType> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.FunctionTypeId, envelope.Command.Name))
            throw new NameNotUnique();

        var functionType = Session.Get<FunctionType>(envelope.Command.FunctionTypeId);

        functionType.Update(envelope.Command.Name);
        await Session.Commit(envelope.User);
    }
}
