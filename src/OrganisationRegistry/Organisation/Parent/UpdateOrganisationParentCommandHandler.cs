namespace OrganisationRegistry.Organisation;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateOrganisationParentCommandHandler
    :BaseCommandHandler<UpdateOrganisationParentCommandHandler>
,ICommandEnvelopeHandler<UpdateOrganisationParent>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateOrganisationParentCommandHandler(ILogger<UpdateOrganisationParentCommandHandler> logger, ISession session, IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationParent> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User,Session)
            .WithVlimpersPolicy()
            .Handle(
                session =>
                {
                    var parentOrganisation = session.Get<Organisation>(envelope.Command.ParentOrganisationId);
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var validity = new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo));

                    if (ParentTreeHasOrganisationInIt(organisation, validity, parentOrganisation))
                        throw new CircularRelationshipDetected();

                    organisation.UpdateParent(
                        envelope.Command.OrganisationOrganisationParentId,
                        parentOrganisation,
                        validity,
                        _dateTimeProvider);
                });

    private bool ParentTreeHasOrganisationInIt(
        Organisation organisation,
        Period validity,
        Organisation parentOrganisation,
        ICollection<Organisation>? alreadyCheckedOrganisations = null)
    {
        alreadyCheckedOrganisations ??= new List<Organisation>();

        if (Equals(organisation, parentOrganisation))
            return true;

        var parentsInPeriod = parentOrganisation.ParentsInPeriod(validity).ToList();

        if (!parentsInPeriod.Any())
            return false;

        return parentsInPeriod
            .Select(parent => Session.Get<Organisation>(parent.ParentOrganisationId))
            .Where(organisation1 => !alreadyCheckedOrganisations.Contains(organisation1))
            .Any(
                organisation1 => ParentTreeHasOrganisationInIt(
                    organisation,
                    validity,
                    organisation1,
                    alreadyCheckedOrganisations.Concat(new List<Organisation> { parentOrganisation }).ToList()));
    }
}
