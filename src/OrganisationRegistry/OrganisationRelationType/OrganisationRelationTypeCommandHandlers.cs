namespace OrganisationRegistry.OrganisationRelationType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class OrganisationRelationTypeCommandHandlers :
    BaseCommandHandler<OrganisationRelationTypeCommandHandlers>,
    ICommandEnvelopeHandler<CreateOrganisationRelationType>,
    ICommandEnvelopeHandler<UpdateOrganisationRelationType>
{
    private readonly IUniqueNameValidator<OrganisationRelationType> _uniqueNameValidator;

    public OrganisationRelationTypeCommandHandlers(
        ILogger<OrganisationRelationTypeCommandHandlers> logger,
        ISession session,
        IUniqueNameValidator<OrganisationRelationType> uniqueNameValidator) : base(logger, session)
    {
        _uniqueNameValidator = uniqueNameValidator;
    }

    public async Task Handle(ICommandEnvelope<CreateOrganisationRelationType> envelope)
        => await Handler.For(envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
                        throw new NameNotUnique();

                    var organisationRelationType = new OrganisationRelationType(envelope.Command.OrganisationRelationTypeId, envelope.Command.Name, envelope.Command.InverseName);
                    session.Add(organisationRelationType);
                });

    public async Task Handle(ICommandEnvelope<UpdateOrganisationRelationType> envelope)
        => await UpdateHandler<OrganisationRelationType>.For(envelope.Command, envelope.User, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueNameValidator.IsNameTaken(envelope.Command.OrganisationRelationTypeId, envelope.Command.Name))
                        throw new NameNotUnique();

                    var organisationRelationType = session.Get<OrganisationRelationType>(envelope.Command.OrganisationRelationTypeId);
                    organisationRelationType.Update(envelope.Command.Name, envelope.Command.InverseName);
                });
}
