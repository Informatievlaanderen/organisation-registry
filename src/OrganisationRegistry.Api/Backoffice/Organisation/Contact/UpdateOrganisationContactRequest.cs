namespace OrganisationRegistry.Api.Backoffice.Organisation.Contact;

using System;
using ContactType;
using FluentValidation;
using OrganisationRegistry.Organisation;
using SqlServer.Organisation;

public class UpdateOrganisationContactInternalRequest
{
    public Guid OrganisationId { get; set; }
    public UpdateOrganisationContactRequest Body { get; }

    public UpdateOrganisationContactInternalRequest(Guid organisationId, UpdateOrganisationContactRequest message)
    {
        OrganisationId = organisationId;
        Body = message;
    }
}

public class UpdateOrganisationContactRequest
{
    public Guid OrganisationContactId { get; set; }
    public Guid ContactTypeId { get; set; }
    public string ContactValue { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateOrganisationContactInternalRequestValidator : AbstractValidator<UpdateOrganisationContactInternalRequest>
{
    public UpdateOrganisationContactInternalRequestValidator()
    {
        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WithMessage("Organisation Id is required.");

        RuleFor(x => x.Body.ContactTypeId)
            .NotEmpty()
            .WithMessage("Contact Type Id is required.");

        RuleFor(x => x.Body.ContactValue)
            .NotEmpty()
            .WithMessage("Contact Value is required.");

        RuleFor(x => x.Body.ContactValue)
            .Length(0, OrganisationContactListConfiguration.ContactValueLength)
            .WithMessage($"Contact Value cannot be longer than {OrganisationContactListConfiguration.ContactValueLength}.");

        RuleFor(x => x.Body.ValidTo)
            .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
            .When(x => x.Body.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");
    }
}

public static class UpdateOrganisationContactRequestMapping
{
    public static UpdateOrganisationContact Map(UpdateOrganisationContactInternalRequest message)
        => new(
            message.Body.OrganisationContactId,
            new OrganisationId(message.OrganisationId),
            new ContactTypeId(message.Body.ContactTypeId),
            message.Body.ContactValue,
            new ValidFrom(message.Body.ValidFrom),
            new ValidTo(message.Body.ValidTo));
}
