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
    /// <summary>
    /// Bankrekeningnummer van de organisatie.
    /// </summary>
    public string BankAccountNumber { get; set; } = null!;
    /// <summary>
    /// Geeft aan of het bankrekeningnummer een geldig Iban formaat heeft.<br />
    /// Indien niet meegegeven, is de standaard waarde 'true'.<br />
    /// Indien deze waarde 'true' is, wordt een foutmelding teruggegeven bij een ongeldig Iban formaat.
    /// </summary>
    public bool IsIban { get; set; } = true;
    /// <summary>
    /// Optioneel: Bic van het bankrekeningnummer.<br />
    /// Indien deze waarde is ingevuld, wordt een foutmelding teruggegeven bij een ongeldig Bic formaat.
    /// </summary>
    public string? Bic { get; set; }
    /// <summary>
    /// Geldig vanaf.
    /// </summary>
    public DateTime? ValidFrom { get; set; }
    /// <summary>
    /// Geldig tot.
    /// </summary>
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
