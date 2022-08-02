namespace OrganisationRegistry.FormalFrameworkCategory;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class FormalFrameworkCategoryCommandHandlers :
    BaseCommandHandler<FormalFrameworkCategoryCommandHandlers>,
    ICommandEnvelopeHandler<CreateFormalFrameworkCategory>,
    ICommandEnvelopeHandler<UpdateFormalFrameworkCategory>
{
    private readonly IUniqueNameValidator<FormalFrameworkCategory> _uniqueNameValidator;

    public FormalFrameworkCategoryCommandHandlers(
        ILogger<FormalFrameworkCategoryCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<FormalFrameworkCategory> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateFormalFrameworkCategory> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var formalFrameworkCategory = new FormalFrameworkCategory(envelope.Command.FormalFrameworkCategoryId, envelope.Command.Name);
                    session.Add(formalFrameworkCategory);
                });

    public async Task Handle(ICommandEnvelope<UpdateFormalFrameworkCategory> envelope)
        => await UpdateHandler<FormalFrameworkCategory>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.FormalFrameworkCategoryId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var formalFrameworkCategory = session.Get<FormalFrameworkCategory>(envelope.Command.FormalFrameworkCategoryId);
                    formalFrameworkCategory.Update(envelope.Command.Name);
                });
}
