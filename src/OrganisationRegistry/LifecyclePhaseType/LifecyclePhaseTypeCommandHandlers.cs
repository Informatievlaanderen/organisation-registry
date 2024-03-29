﻿namespace OrganisationRegistry.LifecyclePhaseType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class LifecyclePhaseTypeCommandHandlers :
    BaseCommandHandler<LifecyclePhaseTypeCommandHandlers>,
    ICommandEnvelopeHandler<CreateLifecyclePhaseType>,
    ICommandEnvelopeHandler<UpdateLifecyclePhaseType>
{
    private readonly IUniqueNameValidator<LifecyclePhaseType> _uniqueNameValidator;
    private readonly IOnlyOneDefaultLifecyclePhaseTypeValidator _onlyOneDefaultLifecyclePhaseTypeValidator;

    public LifecyclePhaseTypeCommandHandlers(
        ILogger<LifecyclePhaseTypeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<LifecyclePhaseType> uniqueNameValidator,
        IOnlyOneDefaultLifecyclePhaseTypeValidator onlyOneDefaultLifecyclePhaseTypeValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
        _onlyOneDefaultLifecyclePhaseTypeValidator = onlyOneDefaultLifecyclePhaseTypeValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateLifecyclePhaseType> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    if (_onlyOneDefaultLifecyclePhaseTypeValidator.ViolatesOnlyOneDefaultLifecyclePhaseTypeConstraint(envelope.Command.LifecyclePhaseTypeIsRepresentativeFor, envelope.Command.Status))
                        throw new DefaultLifecyclePhaseAlreadyPresent(envelope.Command.LifecyclePhaseTypeIsRepresentativeFor);

                    var lifecyclePhaseType = new LifecyclePhaseType(
                        envelope.Command.LifecyclePhaseTypeId,
                        envelope.Command.Name,
                        envelope.Command.LifecyclePhaseTypeIsRepresentativeFor,
                        envelope.Command.Status);

                    session.Add(lifecyclePhaseType);
                });

    public async Task Handle(ICommandEnvelope<UpdateLifecyclePhaseType> envelope)
        => await UpdateHandler<LifecyclePhaseType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.LifecyclePhaseTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    if (_onlyOneDefaultLifecyclePhaseTypeValidator.ViolatesOnlyOneDefaultLifecyclePhaseTypeConstraint(envelope.Command.LifecyclePhaseTypeId, envelope.Command.LifecyclePhaseTypeIsRepresentativeFor, envelope.Command.Status))
                        throw new DefaultLifecyclePhaseAlreadyPresent(envelope.Command.LifecyclePhaseTypeIsRepresentativeFor);

                    var lifecyclePhaseType = session.Get<LifecyclePhaseType>(envelope.Command.LifecyclePhaseTypeId);

                    lifecyclePhaseType.Update(
                        envelope.Command.Name,
                        envelope.Command.LifecyclePhaseTypeIsRepresentativeFor,
                        envelope.Command.Status);
                });
}
