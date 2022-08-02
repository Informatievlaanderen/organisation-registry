namespace OrganisationRegistry.OrganisationClassificationType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class OrganisationClassificationTypeCommandHandlers :
    BaseCommandHandler<OrganisationClassificationTypeCommandHandlers>,
    ICommandEnvelopeHandler<CreateOrganisationClassificationType>,
    ICommandEnvelopeHandler<UpdateOrganisationClassificationType>
{
    private readonly IUniqueNameValidator<OrganisationClassificationType> _uniqueNameValidator;

    public OrganisationClassificationTypeCommandHandlers(
        ILogger<OrganisationClassificationTypeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<OrganisationClassificationType> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateOrganisationClassificationType> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var organisationClassificationType = OrganisationClassificationType.Create(envelope.Command.OrganisationClassificationTypeId, envelope.Command.Name, envelope.Command.AllowDifferentClassificationsToOverlap);
                    session.Add(organisationClassificationType);
                });

    public async Task Handle(ICommandEnvelope<UpdateOrganisationClassificationType> envelope)
        => await UpdateHandler<OrganisationClassificationType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.OrganisationClassificationTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var organisationClassificationType = session.Get<OrganisationClassificationType>(envelope.Command.OrganisationClassificationTypeId);
                    organisationClassificationType.Update(envelope.Command.Name);

                    if (envelope.Command.AllowDifferentClassificationsToOverlap is { } allowDifferentClassificationsToOverlap)
                        organisationClassificationType.ChangeAllowDifferentClassificationsToOverlap(allowDifferentClassificationsToOverlap);
                });
}
