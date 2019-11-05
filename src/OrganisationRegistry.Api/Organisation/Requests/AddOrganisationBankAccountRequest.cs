namespace OrganisationRegistry.Api.Organisation.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

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
        public Guid OrganisationBankAccountId { get; set; }
        public string BankAccountNumber { get; set; }
        public bool IsIban { get; set; }
        public string Bic { get; set; }
        public bool IsBic { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddOrganisationBankAccountInternalRequestValidator : AbstractValidator<AddOrganisationBankAccountInternalRequest>
    {
        public AddOrganisationBankAccountInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.BankAccountNumber)
                .NotEmpty()
                .WithMessage("Bank Account Number is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");
        }
    }

    public static class AddOrganisationBankAccountRequestMapping
    {
        public static AddOrganisationBankAccount Map(AddOrganisationBankAccountInternalRequest message)
        {
            return new AddOrganisationBankAccount(
                message.Body.OrganisationBankAccountId,
                new OrganisationId(message.OrganisationId),
                message.Body.BankAccountNumber,
                message.Body.IsIban,
                message.Body.Bic,
                message.Body.IsBic,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
