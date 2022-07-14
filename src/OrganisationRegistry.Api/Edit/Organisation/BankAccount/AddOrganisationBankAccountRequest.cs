namespace OrganisationRegistry.Api.Edit.Organisation.BankAccount;

using System;
using FluentValidation;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Organisation;

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

public class AddOrganisationBankAccountInternalRequestValidator : AbstractValidator<AddOrganisationBankAccountRequest>
{
    public AddOrganisationBankAccountInternalRequestValidator()
    {
        RuleFor(x => x.BankAccountNumber)
            .NotEmpty()
            .WithMessage("Bank Account Number is required.");

        RuleFor(x => x.ValidTo)
            .GreaterThanOrEqualTo(x => x.ValidFrom)
            .When(x => x.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");
    }
}

public static class AddOrganisationBankAccountRequestMapping
{
    public static AddOrganisationBankAccount Map(Guid organisationId, AddOrganisationBankAccountRequest message)
        => new(
            Guid.NewGuid(),
            new OrganisationId(organisationId),
            message.BankAccountNumber,
            message.IsIban,
            message.Bic,
            message.Bic is { } bic && bic.IsNotEmptyOrWhiteSpace(),
            new ValidFrom(message.ValidFrom),
            new ValidTo(message.ValidTo));
}
