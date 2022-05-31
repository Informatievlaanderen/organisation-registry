namespace OrganisationRegistry.OrganisationClassificationType;

using System.Threading.Tasks;
using Commands;
using Exceptions;
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
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.Name))
            throw new NameNotUnique();

        var organisationClassificationType = new OrganisationClassificationType(envelope.Command.OrganisationClassificationTypeId, envelope.Command.Name);
        Session.Add(organisationClassificationType);
        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<UpdateOrganisationClassificationType> envelope)
    {
        if (_uniqueNameValidator.IsNameTaken(envelope.Command.OrganisationClassificationTypeId, envelope.Command.Name))
            throw new NameNotUnique();

        var organisationClassificationType = Session.Get<OrganisationClassificationType>(envelope.Command.OrganisationClassificationTypeId);
        organisationClassificationType.Update(envelope.Command.Name);
        await Session.Commit(envelope.User);
    }
}
