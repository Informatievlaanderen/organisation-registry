namespace OrganisationRegistry.Organisation;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exceptions;
using FormalFramework;
using Handling;
using Handling.Authorization;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

public class UpdateOrganisationFormalFrameworkCommandHandler
:BaseCommandHandler<UpdateOrganisationFormalFrameworkCommandHandler>
,ICommandEnvelopeHandler<UpdateOrganisationFormalFramework>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateOrganisationFormalFrameworkCommandHandler(ILogger<UpdateOrganisationFormalFrameworkCommandHandler> logger, ISession session, IDateTimeProvider dateTimeProvider, IOrganisationRegistryConfiguration organisationRegistryConfiguration) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<UpdateOrganisationFormalFramework> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command,envelope.User, Session)
            .WithPolicy(
                organisation => new FormalFrameworkPolicy(
                    () => organisation.State.OvoNumber,
                    envelope.Command.FormalFrameworkId,
                    _organisationRegistryConfiguration))
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var formalFramework = session.Get<FormalFramework>(envelope.Command.FormalFrameworkId);
                    var parentOrganisation = session.Get<Organisation>(envelope.Command.ParentOrganisationId);

                    var validity = new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo));

                    if (FormalFrameworkTreeHasOrganisationInIt(
                            organisation,
                            formalFramework,
                            validity,
                            parentOrganisation,
                            new List<Organisation>()))
                        throw new CircularRelationInFormalFramework();

                    organisation.UpdateFormalFramework(
                        envelope.Command.OrganisationFormalFrameworkId,
                        formalFramework,
                        parentOrganisation,
                        validity,
                        _dateTimeProvider);
                });

    private bool FormalFrameworkTreeHasOrganisationInIt(
        Organisation organisation,
        FormalFramework formalFramework,
        Period validity,
        Organisation parentOrganisation,
        IEnumerable<Organisation> alreadyCheckedOrganisations)
    {
        if (Equals(organisation, parentOrganisation))
            return true;

        var parentsInPeriodForFormalFramework = parentOrganisation.ParentsInPeriod(formalFramework, validity).ToList();

        if (!parentsInPeriodForFormalFramework.Any())
            return false;

        return parentsInPeriodForFormalFramework
            .Select(parent => Session.Get<Organisation>(parent.ParentOrganisationId))
            .Where(organisation1 => !alreadyCheckedOrganisations.Contains(organisation1))
            .Any(
                organisation1 => FormalFrameworkTreeHasOrganisationInIt(
                    organisation,
                    formalFramework,
                    validity,
                    organisation1,
                    alreadyCheckedOrganisations.Concat(new List<Organisation> { parentOrganisation }).ToList()));
    }
}
