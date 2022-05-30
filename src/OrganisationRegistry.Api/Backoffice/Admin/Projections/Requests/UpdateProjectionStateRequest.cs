namespace OrganisationRegistry.Api.Backoffice.Admin.Projections.Requests;

using System;
using FluentValidation;
using SqlServer.ProjectionState;

public class UpdateProjectionStateInternalRequest
{
    public Guid ProjectionStateId { get; }
    public UpdateProjectionStateRequest Body { get; }

    public UpdateProjectionStateInternalRequest(Guid projectionStateId, UpdateProjectionStateRequest body)
    {
        ProjectionStateId = projectionStateId;
        Body = body;
    }
}

public class UpdateProjectionStateRequest
{
    public string Name { get; init; } = null!;

    public int EventNumber { get; init; }
}

public class UpdateProjectionStateRequestValidator : AbstractValidator<UpdateProjectionStateInternalRequest>
{
    public UpdateProjectionStateRequestValidator()
    {
        RuleFor(x => x.ProjectionStateId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Body.Name)
            .Length(0, ProjectionStateListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {ProjectionStateListConfiguration.NameLength}.");

        RuleFor(x => x.Body.EventNumber)
            .NotEmpty()
            .WithMessage("EventNumber is required.");
    }
}
