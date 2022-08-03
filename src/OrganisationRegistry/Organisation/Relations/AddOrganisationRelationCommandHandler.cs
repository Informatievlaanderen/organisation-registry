namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using OrganisationRelationType;

public class AddOrganisationRelationCommandHandler
    : BaseCommandHandler<AddOrganisationRelationCommandHandler>,
        ICommandEnvelopeHandler<AddOrganisationRelation>
{
    public AddOrganisationRelationCommandHandler(ILogger<AddOrganisationRelationCommandHandler> logger, ISession session) : base(logger, session)
    {
    }

    public Task Handle(ICommandEnvelope<AddOrganisationRelation> envelope)
        => UpdateHandler<Organisation>.For(envelope.Command, envelope.User, Session)
            .WithBeheerderForOrganisationPolicy()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(envelope.Command.OrganisationId);
                    organisation.ThrowIfTerminated(envelope.User);

                    var relatedOrganisation = session.Get<Organisation>(envelope.Command.RelatedOrganisationId);
                    var relation = session.Get<OrganisationRelationType>(envelope.Command.RelationTypeId);

                    organisation.AddRelation(
                        envelope.Command.OrganisationRelationId,
                        relation,
                        relatedOrganisation,
                        new Period(new ValidFrom(envelope.Command.ValidFrom), new ValidTo(envelope.Command.ValidTo)));
                });
}
