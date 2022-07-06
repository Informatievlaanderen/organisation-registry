namespace OrganisationRegistry.Api.Edit.Organisation.BankAccount;

using System;
using FluentValidation;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Organisation;

public class AddOrganisationBankAccountInternalRequest
{
    public Guid OrganisationId { get; set; }
    public AddOrganisationBankAccountRequest Body { get; }

    public AddOrganisationBankAccountInternalRequest(Guid organisationId, AddOrganisationBankAccountRequest message)
    {
        OrganisationId = organisationId;
        Body = message;
    }
}

public class AddOrganisationBankAccountRequest
{
    public string BankAccountNumber { get; set; } = null!;
    public bool IsIban { get; set; }
    public string? Bic { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class AddOrganisationBankAccountInternalRequestValidator : AbstractValidator<AddOrganisationBankAccountInternalRequest>
{
    public AddOrganisationBankAccountInternalRequestValidator()
    {
        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WithMessage("Organisation Id is required.");

        RuleFor(x => x.Body.BankAccountNumber)
            .NotEmpty()
            .WithMessage("Bank Account Number is required.");

        RuleFor(x => x.Body.ValidTo)
            .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
            .When(x => x.Body.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");
    }
}

public static class AddOrganisationBankAccountRequestMapping
{
    public static AddOrganisationBankAccount Map(AddOrganisationBankAccountInternalRequest message)
        => new(
            Guid.NewGuid(),
            new OrganisationId(message.OrganisationId),
            message.Body.BankAccountNumber,
            message.Body.IsIban,
            message.Body.Bic,
            message.Body.Bic is { } bic && bic.IsNotEmptyOrWhiteSpace(),
            new ValidFrom(message.Body.ValidFrom),
            new ValidTo(message.Body.ValidTo));
}
