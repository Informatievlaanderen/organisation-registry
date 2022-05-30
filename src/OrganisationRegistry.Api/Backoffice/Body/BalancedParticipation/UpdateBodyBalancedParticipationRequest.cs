namespace OrganisationRegistry.Api.Backoffice.Body.BalancedParticipation;

using System;
using FluentValidation;
using OrganisationRegistry.Body;

public class UpdateBodyBalancedParticipationInternalRequest
{
    public Guid BodyId { get; set; }
    public UpdateBodyBalancedParticipationRequest Body { get; set; }

    public UpdateBodyBalancedParticipationInternalRequest(Guid bodyId, UpdateBodyBalancedParticipationRequest body)
    {
        BodyId = bodyId;
        Body = body;
    }
}

public class UpdateBodyBalancedParticipationRequest
{
    public bool? Obligatory { get; set; }

    public string? ExtraRemark { get; set; }

    public string? ExceptionMeasure { get; set; }

}

public class UpdateBodyBalancedParticipationRequestValidator : AbstractValidator<UpdateBodyBalancedParticipationInternalRequest>
{
    public UpdateBodyBalancedParticipationRequestValidator()
    {
        RuleFor(x => x.BodyId)
            .NotEmpty()
            .WithMessage("Id is required.");
    }
}

public static class UpdateBodyBalancedParticipationRequestMapping
{
    public static UpdateBodyBalancedParticipation Map(UpdateBodyBalancedParticipationInternalRequest message)
    {
        return new UpdateBodyBalancedParticipation(
            new BodyId(message.BodyId),
            message.Body.Obligatory,
            message.Body.ExtraRemark,
            message.Body.ExceptionMeasure);
    }
}