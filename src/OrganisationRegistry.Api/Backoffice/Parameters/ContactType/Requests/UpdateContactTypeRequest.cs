﻿namespace OrganisationRegistry.Api.Backoffice.Parameters.ContactType.Requests;

using System;
using System.Text.RegularExpressions;
using FluentValidation;
using OrganisationRegistry.ContactType;
using OrganisationRegistry.ContactType.Commands;
using SqlServer.ContactType;

public class UpdateContactTypeInternalRequest
{
    public Guid ContactTypeId { get; set; }
    public UpdateContactTypeRequest Body { get; set; }

    public UpdateContactTypeInternalRequest(Guid contactTypeId, UpdateContactTypeRequest body)
    {
        ContactTypeId = contactTypeId;
        Body = body;
    }
}

public class UpdateContactTypeRequest
{
    public string Name { get; set; } = null!;

    public string Regex { get; set; } = null!;

    public string Example { get; set; } = null!;
}

public class UpdateContactTypeRequestValidator : AbstractValidator<UpdateContactTypeInternalRequest>
{
    public UpdateContactTypeRequestValidator()
    {
        RuleFor(x => x.ContactTypeId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Body.Regex)
            .NotEmpty()
            .WithMessage("Regex is required.");

        RuleFor(x => x.Body.Regex)
            .SetValidator(new RegexValidator<UpdateContactTypeInternalRequest>())
            .WithMessage("Regular expression must be valid.");

        RuleFor(x => x.Body.Example)
            .NotEmpty()
            .WithMessage("Example is required.");

        RuleFor(x => x.Body.Name)
            .Length(0, ContactTypeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {ContactTypeListConfiguration.NameLength}.");
    }
}

public static class UpdateContactTypeRequestMapping
{
    public static UpdateContactType Map(UpdateContactTypeInternalRequest message)
        => new(
            new ContactTypeId(message.ContactTypeId),
            message.Body.Name,
            new Regex(message.Body.Regex),
            message.Body.Example);
}
