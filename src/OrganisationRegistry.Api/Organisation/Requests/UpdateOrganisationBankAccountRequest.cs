namespace OrganisationRegistry.Api.Organisation.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    public class UpdateOrganisationBankAccountInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public UpdateOrganisationBankAccountRequest Body { get; }

        public UpdateOrganisationBankAccountInternalRequest(Guid organisationId, UpdateOrganisationBankAccountRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class UpdateOrganisationBankAccountRequest
    {
        public Guid OrganisationBankAccountId { get; set; }
        public string BankAccountNumber { get; set; }
        public bool IsIban { get; set; }
        public string Bic { get; set; }
        public bool IsBic { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class UpdateOrganisationBankAccountInternalRequestValidator : AbstractValidator<UpdateOrganisationBankAccountInternalRequest>
    {
        public UpdateOrganisationBankAccountInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.BankAccountNumber)
                .NotEmpty()
                .WithMessage("Bank Account Number is required.");

            // TODO: Validate if BankAccountId is valid

            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From");

            // TODO: Validate if org id is valid
        }
    }

    public static class UpdateOrganisationBankAccountRequestMapping
    {
        public static UpdateOrganisationBankAccount Map(UpdateOrganisationBankAccountInternalRequest message)
        {
            return new UpdateOrganisationBankAccount(
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
