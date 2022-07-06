namespace OrganisationRegistry.Api.Edit.Organisation.BankAccount;

using System;
using FluentValidation;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Organisation;

public class UpdateOrganisationBankAccountInternalRequest
{
    public Guid OrganisationId { get; set; }
    public Guid OrganisationBankAccountId { get; set; }

    public UpdateOrganisationBankAccountRequest Body { get; }

    public UpdateOrganisationBankAccountInternalRequest(Guid organisationId, Guid organisationBankAccountId, UpdateOrganisationBankAccountRequest message)
    {
        OrganisationId = organisationId;
        OrganisationBankAccountId = organisationBankAccountId;
        Body = message;
    }
}

public class UpdateOrganisationBankAccountRequest
{
    public string BankAccountNumber { get; set; } = null!;
    public bool IsIban { get; set; }
    public string? Bic { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class UpdateOrganisationBankAccountInternalRequestValidator : AbstractValidator<UpdateOrganisationBankAccountInternalRequest>
{
    public UpdateOrganisationBankAccountInternalRequestValidator()
    {
        RuleFor(x => x.OrganisationId)
            .NotEmpty()
            .WithMessage("Organisation Id is required.");

        RuleFor(x => x.Body.BankAccountNumber)
            .NotEmpty()
            .WithMessage("Bank Account Number is required.");

        RuleFor(x => x.OrganisationBankAccountId)
            .NotEmpty()
            .WithMessage("Organisation Bank Account Id is required.");

        RuleFor(x => x.Body.ValidTo)
            .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
            .When(x => x.Body.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From");
    }
}

public static class UpdateOrganisationBankAccountRequestMapping
{
    public static UpdateOrganisationBankAccount Map(UpdateOrganisationBankAccountInternalRequest message)
        => new(
            message.OrganisationBankAccountId,
            new OrganisationId(message.OrganisationId),
            message.Body.BankAccountNumber,
            message.Body.IsIban,
            message.Body.Bic,
            message.Body.Bic is { } bic && bic.IsNotEmptyOrWhiteSpace(),
            new ValidFrom(message.Body.ValidFrom),
            new ValidTo(message.Body.ValidTo));
}
