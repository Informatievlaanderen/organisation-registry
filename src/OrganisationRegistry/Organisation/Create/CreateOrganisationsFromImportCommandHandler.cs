namespace OrganisationRegistry.Organisation;

using System;
using System.Threading.Tasks;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;

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
            .HandleWithCombinedTransaction(
                session =>
                {
                    foreach (var record in envelope.Command.Records)
                    {
                        var parent = session.Get<Organisation>(record.ParentOrganisationId);

                        var ovoNumber = _ovoNumberGenerator.GenerateNextNumber();
                        var validFrom = record.Validity_Start.HasValue
                            ? new ValidFrom(record.Validity_Start.Value.ToDateTime(new TimeOnly()))
                            : new ValidFrom();

                        var operationalValidFrom = record.OperationalValidity_Start.HasValue
                            ? new ValidFrom(record.OperationalValidity_Start.Value.ToDateTime(new TimeOnly()))
                            : new ValidFrom();

                        var organisation = Organisation.CreateFromImport(
                            OrganisationId.New(),
                            record.Name,
                            ovoNumber,
                            record.ShortName,
                            record.Article ?? Article.None,
                            parent,
                            string.Empty,
                            new Period(validFrom, new ValidTo()),
                            new Period(operationalValidFrom, new ValidTo()),
                            _dateTimeProvider,
                            new OrganisationSourceId(envelope.Command.ImportFileId),
                            record.Reference);

                        session.Add(organisation);
                    }
                });
    }
}
