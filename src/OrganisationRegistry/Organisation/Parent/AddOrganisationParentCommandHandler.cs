namespace OrganisationRegistry.Organisation;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exceptions;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class AddOrganisationParentCommandHandler
    : BaseCommandHandler<AddOrganisationParentCommandHandler>
        , ICommandEnvelopeHandler<AddOrganisationParent>
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public AddOrganisationParentCommandHandler(
        ILogger<AddOrganisationParentCommandHandler> logger,
        ISession session,
        IDateTimeProvider dateTimeProvider) : base(logger, session)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<AddOrganisationParent> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithVlimpersPolicy()
            .Handle(
                session =>
                {
                    var parentOrganisation = session.Get<Organisation>(envelope.Command.ParentOrganisationId);
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var validity = new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo));

                    ThrowIfCircularRelationshipDetected(organisation, validity, parentOrganisation);

                    organisation.AddParent(
                        envelope.Command.OrganisationOrganisationParentId,
                        parentOrganisation,
                        validity,
                        _dateTimeProvider);
                });

    private void ThrowIfCircularRelationshipDetected(
        Organisation organisation,
        Period validity,
        Organisation parentOrganisation)
    {
        var parentTreeHasOrganisationInIt =
            ParentTreeHasOrganisationInIt(
                organisation,
                validity,
                parentOrganisation);

        if (parentTreeHasOrganisationInIt)
            throw new CircularRelationshipDetected();
    }

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
