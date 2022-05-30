﻿namespace OrganisationRegistry.Api.Backoffice.Body.Organisation;

using System;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using OrganisationRegistry.Body;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation;

public class AddBodyOrganisationInternalRequest
{
    public Guid BodyId { get; set; }
    public AddBodyOrganisationRequest Body { get; }

    public AddBodyOrganisationInternalRequest(Guid bodyId, AddBodyOrganisationRequest message)
    {
        BodyId = bodyId;
        Body = message;
    }
}

public class AddBodyOrganisationRequest
{
    public Guid BodyOrganisationId { get; set; }
    public Guid OrganisationId { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class AddBodyOrganisationInternalRequestValidator : AbstractValidator<AddBodyOrganisationInternalRequest>
{
    public AddBodyOrganisationInternalRequestValidator(IHttpContextAccessor httpContextAccessor, ISecurityService securityService)
    {
        RuleFor(x => x.BodyId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.BodyOrganisationId)
            .NotEmpty()
            .WithMessage("Body Organisation Id is required.");

        RuleFor(x => x.Body.OrganisationId)
            .NotEmpty()
            .WithMessage("Organisation Id is required.");

        RuleFor(x => x.Body.ValidTo)
            .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
            .When(x => x.Body.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");

        RuleFor(x => x.Body.OrganisationId)
            .NotEmpty()
            .WhenAsync(async (_, _) => await httpContextAccessor.UserIsDecentraalBeheerder(securityService.GetSecurityInformation))
            .WithMessage("Organisation Id is required for users in role 'organisatieBeheerder'.");
    }
}

public static class AddBodyOrganisationRequestMapping
{
    public static AddBodyOrganisation Map(AddBodyOrganisationInternalRequest message)
        => new(
            new BodyId(message.BodyId),
            new BodyOrganisationId(message.Body.BodyOrganisationId),
            new OrganisationId(message.Body.OrganisationId),
            new Period(
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo)));
}