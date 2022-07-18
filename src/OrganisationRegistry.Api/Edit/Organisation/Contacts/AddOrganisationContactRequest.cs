namespace OrganisationRegistry.Api.Edit.Organisation.Contacts;

using System;
using ContactType;
using FluentValidation;
using OrganisationRegistry.Organisation;
using SqlServer.Organisation;

public class AddOrganisationContactRequest
{
    public Guid ContactTypeId { get; set; }
    public string ContactValue { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class AddOrganisationContactInternalRequestValidator : AbstractValidator<AddOrganisationContactRequest>
{
    public AddOrganisationContactInternalRequestValidator()
    {
        RuleFor(x => x.ContactTypeId)
            .NotEmpty()
            .WithMessage("Contact Type Id is required.");

        RuleFor(x => x.ContactValue)
            .NotEmpty()
            .WithMessage("Contact Value is required.");

        RuleFor(x => x.ContactValue)
            .Length(0, OrganisationContactListConfiguration.ContactValueLength)
            .WithMessage($"Contact Value cannot be longer than {OrganisationContactListConfiguration.ContactValueLength}.");

        RuleFor(x => x.ValidTo)
            .GreaterThanOrEqualTo(x => x.ValidFrom)
            .When(x => x.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");
    }
}

public static class AddOrganisationContactRequestMapping
{
    public static AddOrganisationContact Map(Guid organisationId, AddOrganisationContactRequest message)
        => new(
            Guid.NewGuid(),
            new OrganisationId(organisationId),
            new ContactTypeId(message.ContactTypeId),
            message.ContactValue,
            new ValidFrom(message.ValidFrom),
            new ValidTo(message.ValidTo));
}
