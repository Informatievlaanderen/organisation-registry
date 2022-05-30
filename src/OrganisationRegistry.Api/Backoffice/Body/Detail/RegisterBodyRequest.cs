namespace OrganisationRegistry.Api.Backoffice.Body.Detail;

using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using OrganisationRegistry.Body;
using OrganisationRegistry.Infrastructure.Authorization;
using LifecyclePhaseType;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.SqlServer.Body;
using OrganisationRegistry.SqlServer.LifecyclePhaseType;

public class RegisterBodyRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? BodyNumber { get; set; }

    public string? ShortName { get; set; }

    public Guid? OrganisationId { get; set; }

    public string? Description { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }

    public DateTime? FormalValidFrom { get; set; }

    public DateTime? FormalValidTo { get; set; }
}

public class RegisterBodyRequestValidator : AbstractValidator<RegisterBodyRequest>
{
    public RegisterBodyRequestValidator(IHttpContextAccessor httpContextAccessor, ISecurityService securityService)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Name)
            .Length(0, BodyListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {BodyListConfiguration.NameLength}.");

        RuleFor(x => x.BodyNumber)
            .Length(0, BodyListConfiguration.BodyNumberLength)
            .WithMessage($"Name cannot be longer than {BodyListConfiguration.BodyNumberLength}.");

        RuleFor(x => x.ValidTo)
            .GreaterThanOrEqualTo(x => x.ValidFrom)
            .When(x => x.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");

        RuleFor(x => x.FormalValidTo)
            .GreaterThanOrEqualTo(x => x.FormalValidFrom)
            .When(x => x.FormalValidFrom.HasValue)
            .WithMessage("Formal Valid To must be greater than or equal to Formal Valid From.");

        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WhenAsync(async (_, _) => await httpContextAccessor.UserIsDecentraalBeheerder(securityService.GetSecurityInformation))
            .WithMessage("Organisation Id is required for users in role 'organisatieBeheerder'.");
    }
}

public static class RegisterBodyRequestMapping
{
    public static RegisterBody Map(
        RegisterBodyRequest message,
        LifecyclePhaseTypeListItem? activeLifecyclePhaseTypeListItem,
        LifecyclePhaseTypeListItem? inactiveLifecyclePhaseTypeListItem)
        => new(
            new BodyId(message.Id),
            message.Name,
            message.BodyNumber,
            message.ShortName,
            message.OrganisationId != null ? new OrganisationId(message.OrganisationId.Value) : null,
            message.Description,
            new Period(
                new ValidFrom(message.ValidFrom),
                new ValidTo(message.ValidTo)),
            new Period(
                new ValidFrom(message.FormalValidFrom),
                new ValidTo(message.FormalValidTo)),
            activeLifecyclePhaseTypeListItem != null ? new LifecyclePhaseTypeId(activeLifecyclePhaseTypeListItem.Id) : null,
            inactiveLifecyclePhaseTypeListItem != null ? new LifecyclePhaseTypeId(inactiveLifecyclePhaseTypeListItem.Id) : null);
}