namespace OrganisationRegistry.FormalFramework;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using FormalFrameworkCategory;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class FormalFrameworkCommandHandlers :
    BaseCommandHandler<FormalFrameworkCommandHandlers>,
    ICommandEnvelopeHandler<CreateFormalFramework>,
    ICommandEnvelopeHandler<UpdateFormalFramework>
{
    private readonly IUniqueNameWithinTypeValidator<FormalFramework> _uniqueNameValidator;
    private readonly IUniqueCodeValidator<FormalFramework> _uniqueCodeValidator;

    public FormalFrameworkCommandHandlers(
        ILogger<FormalFrameworkCommandHandlers> logger,
        ISession session,
        IUniqueNameWithinTypeValidator<FormalFramework> uniqueNameValidator,
        IUniqueCodeValidator<FormalFramework> uniqueCodeValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
        _uniqueCodeValidator = uniqueCodeValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateFormalFramework> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name, envelope.Command.FormalFrameworkCategoryId))
            throw new NameNotUniqueWithinType();

        if (_uniqueCodeValidator.IsCodeTaken(envelope.Command.Code))
            throw new CodeNotUnique();

        var formalFrameworkCategory = Session.Get<FormalFrameworkCategory>(envelope.Command.FormalFrameworkCategoryId);

        var formalFramework = new FormalFramework(envelope.Command.FormalFrameworkId, envelope.Command.Name, envelope.Command.Code, formalFrameworkCategory);
        Session.Add(formalFramework);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<UpdateFormalFramework> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.FormalFrameworkId, envelope.Command.Name, envelope.Command.FormalFrameworkCategoryId))
            throw new NameNotUniqueWithinType();

        if (_uniqueCodeValidator.IsCodeTaken(envelope.Command.FormalFrameworkId, envelope.Command.Code))
            throw new CodeNotUnique();

        var formalFrameworkCategory = Session.Get<FormalFrameworkCategory>(envelope.Command.FormalFrameworkCategoryId);

        var formalFramework = Session.Get<FormalFramework>(envelope.Command.FormalFrameworkId);
        formalFramework.Update(envelope.Command.Name, envelope.Command.Code, formalFrameworkCategory);
        await Session.Commit(envelope.User);
    }
}