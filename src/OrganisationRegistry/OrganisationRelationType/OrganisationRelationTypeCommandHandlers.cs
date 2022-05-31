namespace OrganisationRegistry.OrganisationRelationType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
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
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
            throw new NameNotUnique();

        var organisationRelationType = new OrganisationRelationType(envelope.Command.OrganisationRelationTypeId, envelope.Command.Name, envelope.Command.InverseName);
        Session.Add(organisationRelationType);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<UpdateOrganisationRelationType> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.OrganisationRelationTypeId, envelope.Command.Name))
            throw new NameNotUnique();

        var organisationRelationType = Session.Get<OrganisationRelationType>(envelope.Command.OrganisationRelationTypeId);
        organisationRelationType.Update(envelope.Command.Name, envelope.Command.InverseName);
        await Session.Commit(envelope.User);
    }
}
