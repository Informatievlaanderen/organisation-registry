namespace OrganisationRegistry.Organisation;

using System;
using System.Linq;
using System.Threading.Tasks;
using Exceptions;
using Handling;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Purpose;

public class CreateOrganisationCommandHandler :
    BaseCommandHandler<CreateOrganisationCommandHandler>,
    ICommandEnvelopeHandler<CreateOrganisation>
{
    private readonly IOvoNumberGenerator _ovoNumberGenerator;
    private readonly IUniqueOvoNumberValidator _uniqueOvoNumberValidator;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateOrganisationCommandHandler(
        ILogger<CreateOrganisationCommandHandler> logger,
        ISession session,
        IOvoNumberGenerator ovoNumberGenerator,
        IUniqueOvoNumberValidator uniqueOvoNumberValidator,
        IDateTimeProvider dateTimeProvider
    ) : base(logger, session)
    {
        _ovoNumberGenerator = ovoNumberGenerator;
        _uniqueOvoNumberValidator = uniqueOvoNumberValidator;
        _dateTimeProvider = dateTimeProvider;
    }

    public Task Handle(ICommandEnvelope<CreateOrganisation> envelope)
        => envelope.Command.ParentOrganisationId is not { } ? CreateTopLevelOrganisation(envelope.User, envelope.Command) : CreateDaughter(envelope.User, envelope.Command);

    private Task CreateTopLevelOrganisation(IUser user, CreateOrganisation message)
    {
        return Handler.For(user, Session)
            .RequiresOneOfRole(Role.AlgemeenBeheerder, Role.CjmBeheerder)
            .Handle(
                session =>
                {
                    if (_uniqueOvoNumberValidator.IsOvoNumberTaken(message.OvoNumber))
                        throw new OvoNumberNotUnique();

                    var ovoNumber = string.IsNullOrWhiteSpace(message.OvoNumber)
                        ? _ovoNumberGenerator.GenerateNumber()
                        : message.OvoNumber;

                    var purposes = message
                        .Purposes
                        .Select(purposeId => session.Get<Purpose>(purposeId))
                        .ToList();

                    var organisation = Organisation.Create(
                        message.OrganisationId,
                        message.Name,
                        ovoNumber,
                        message.ShortName,
                        message.Article,
                        null,
                        message.Description,
                        purposes,
                        message.ShowOnVlaamseOverheidSites,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                        new Period(
                            new ValidFrom(message.OperationalValidFrom),
                            new ValidTo(message.OperationalValidTo)),
                        _dateTimeProvider);

                    session.Add(organisation);
                });
    }

    private Task CreateDaughter(IUser user, CreateOrganisation message)
    {
        if (message.ParentOrganisationId is not { } parentId)
            throw new NullReferenceException("parentOrganisationId should not be null when creating a daughter");

        return Handler.For(user, Session)
            .WithVlimpersPolicy(Session.Get<Organisation>(parentId))
            .Handle(
                session =>
                {
                    var parentOrganisation =
                        message.ParentOrganisationId is { } parentOrganisationId
                            ? session.Get<Organisation>(parentOrganisationId)
                            : null;

                    if (_uniqueOvoNumberValidator.IsOvoNumberTaken(message.OvoNumber))
                        throw new OvoNumberNotUnique();

                    var ovoNumber = string.IsNullOrWhiteSpace(message.OvoNumber)
                        ? _ovoNumberGenerator.GenerateNumber()
                        : message.OvoNumber;

                    var purposes = message
                        .Purposes
                        .Select(purposeId => session.Get<Purpose>(purposeId))
                        .ToList();

                    var organisation = Organisation.Create(
                        message.OrganisationId,
                        message.Name,
                        ovoNumber,
                        message.ShortName,
                        message.Article,
                        parentOrganisation,
                        message.Description,
                        purposes,
                        message.ShowOnVlaamseOverheidSites,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                        new Period(
                            new ValidFrom(message.OperationalValidFrom),
                            new ValidTo(message.OperationalValidTo)),
                        _dateTimeProvider);

                    session.Add(organisation);
                });
    }
}
