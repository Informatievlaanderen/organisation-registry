namespace OrganisationRegistry.Organisation;

using System;
using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Purpose;

public class CreateOrganisationsFromImportCommandHandler :
    BaseCommandHandler<CreateOrganisationsFromImportCommandHandler>,
    ICommandEnvelopeHandler<CreateOrganisationsFromImport>
{
    private readonly IOvoNumberGenerator _ovoNumberGenerator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateOrganisationsFromImportCommandHandler(
        ILogger<CreateOrganisationsFromImportCommandHandler> logger,
        IOvoNumberGenerator ovoNumberGenerator,
        IDateTimeProvider dateTimeProvider,
        ISession session
    ) : base(logger, session)
    {
        _ovoNumberGenerator = ovoNumberGenerator;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<CreateOrganisationsFromImport> envelope)
    {
        return Handler.For(envelope.User, Session)
            .RequiresAdmin()
            .Handle(
                session =>
                {
                    foreach (var record in envelope.Command.Records)
                    {
                        var parent = session.Get<Organisation>(record.ParentOrganisationId);

                        var ovoNumber = _ovoNumberGenerator.GenerateNumber();
                        var validFrom = record.Validity_Start.HasValue
                            ? new ValidFrom(record.Validity_Start.Value.ToDateTime(new TimeOnly()))
                            : new ValidFrom();

                        var operationalValidFrom = record.OperationalValidity_Start.HasValue
                            ? new ValidFrom(record.OperationalValidity_Start.Value.ToDateTime(new TimeOnly()))
                            : new ValidFrom();

                        var organisation = Organisation.Create(
                            OrganisationId.New(),
                            record.Name,
                            ovoNumber,
                            record.ShortName,
                            record.Article ?? Article.None,
                            parent,
                            string.Empty,
                            Array.Empty<Purpose>(),
                            false,
                            new Period(validFrom, new ValidTo()),
                            new Period(operationalValidFrom, new ValidTo()),
                            _dateTimeProvider);

                        session.Add(organisation);
                    }
                });
    }
}
