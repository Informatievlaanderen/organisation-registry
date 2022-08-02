namespace OrganisationRegistry.Body;

using System.Threading.Tasks;
using Exceptions;
using Handling;
using Infrastructure.Commands;
using Infrastructure.Domain;
using LifecyclePhaseType;
using Microsoft.Extensions.Logging;
using Organisation;

public class RegisterBodyCommandHandler
:BaseCommandHandler<RegisterBodyCommandHandler>,
    ICommandEnvelopeHandler<RegisterBody>
{
    private readonly IBodyNumberGenerator _bodyNumberGenerator;
    private readonly IUniqueBodyNumberValidator _uniqueBodyNumberValidator;

    public RegisterBodyCommandHandler(
        ILogger<RegisterBodyCommandHandler> logger,
        ISession session,
        IBodyNumberGenerator bodyNumberGenerator,
        IUniqueBodyNumberValidator uniqueBodyNumberValidator) : base(logger, session)
    {
        _bodyNumberGenerator = bodyNumberGenerator;
        _uniqueBodyNumberValidator = uniqueBodyNumberValidator;
    }

    public async Task Handle(ICommandEnvelope<RegisterBody> envelope)
    {
        await Handler.For(envelope.User, Session)
            .WithRegisterBodyPolicy(envelope.Command.OrganisationId)
            .Handle(
                session =>
                {
                    var bodyNumber = GetBodyNumber(envelope.Command);

                    var organisation =
                        envelope.Command.OrganisationId is { } organisationId
                            ? session.Get<Organisation>(organisationId)
                            : null;

                    var activeLifecyclePhaseType =
                        envelope.Command.ActiveLifecyclePhaseTypeId is { } activeLifecyclePhaseTypeId
                            ? session.Get<LifecyclePhaseType>(activeLifecyclePhaseTypeId)
                            : null;

                    var inActiveLifecyclePhaseType =
                        envelope.Command.InactiveLifecyclePhaseTypeId is { } inactiveLifecyclePhaseTypeId
                            ? session.Get<LifecyclePhaseType>(inactiveLifecyclePhaseTypeId)
                            : null;

                    var body = new Body(
                        envelope.Command.BodyId,
                        envelope.Command.Name,
                        bodyNumber,
                        envelope.Command.ShortName,
                        organisation,
                        envelope.Command.Description,
                        envelope.Command.Validity,
                        envelope.Command.FormalValidity,
                        activeLifecyclePhaseType,
                        inActiveLifecyclePhaseType);

                    session.Add(body);
                });

    }

    private string GetBodyNumber(RegisterBody message)
    {
        if (message.BodyNumber is not { } bodyNumber || string.IsNullOrWhiteSpace(bodyNumber))
            return _bodyNumberGenerator.GenerateNumber();

        if (_uniqueBodyNumberValidator.IsBodyNumberTaken(bodyNumber))
            throw new BodyNumberNotUnique();

        return message.BodyNumber;
    }
}
