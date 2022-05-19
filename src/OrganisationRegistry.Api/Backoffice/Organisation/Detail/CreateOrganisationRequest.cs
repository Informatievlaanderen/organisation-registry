namespace OrganisationRegistry.Api.Backoffice.Organisation.Detail
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using Purpose;
    using OrganisationRegistry.SqlServer.Location;
    using OrganisationRegistry.SqlServer.Organisation;

    public class CreateOrganisationRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? OvoNumber { get; set; }

        public string? ShortName { get; set; }

        public string? Article { get; set; }
        public Guid? ParentOrganisationId { get; set; }

        public List<Guid>? PurposeIds { get; set; }

        public bool ShowOnVlaamseOverheidSites { get; set; }

        public string? Description { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public string? KboNumber { get; set; }

        public List<BankAccount>? BankAccounts { get; set; }

        public List<LegalForm>? LegalForms { get; set; }

        public List<Address>? Addresses { get; set; }
        public DateTime? OperationalValidFrom { get; set; }
        public DateTime? OperationalValidTo { get; set; }
    }

    public class BankAccount
    {
        public string Iban { get; set; } = null!;
        public string Bic { get; set; } = null!;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class LegalForm
    {
        public string Code { get; set; } = null!;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class Address
    {
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string Street { get; set; } = null!;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class CreateOrganisationRequestValidator : AbstractValidator<CreateOrganisationRequest>
    {
        public CreateOrganisationRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .When(x => string.IsNullOrEmpty(x.KboNumber));

            RuleFor(x => x.Name)
                .Length(0, OrganisationListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {OrganisationListConfiguration.NameLength}.");

            RuleFor(x => x.ValidTo)
                .GreaterThanOrEqualTo(x => x.ValidFrom)
                .When(x => x.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleForEach(x => x.BankAccounts).SetValidator(new BankAccountValidator());

            RuleForEach(x => x.LegalForms).SetValidator(new LegalFormValidator());

            RuleForEach(x => x.Addresses).SetValidator(new AddressValidator());
        }
    }

    public class BankAccountValidator : AbstractValidator<BankAccount>
    {
        public BankAccountValidator()
        {
            RuleFor(x => x.Iban)
                .NotEmpty()
                .WithMessage("KBO BankAccount: Iban is required.");

            RuleFor(x => x.Bic)
                .NotEmpty()
                .WithMessage("KBO BankAccount: Bic is required.");

            RuleFor(x => x.ValidTo)
                .GreaterThanOrEqualTo(x => x.ValidFrom)
                .When(x => x.ValidFrom.HasValue)
                .WithMessage("KBO BankAccount: Valid To must be greater than or equal to Valid From.");
        }
    }

    public class LegalFormValidator : AbstractValidator<LegalForm>
    {
        public LegalFormValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("KBO LegalForm: Code is required.");

            RuleFor(x => x.ValidTo)
                .GreaterThanOrEqualTo(x => x.ValidFrom)
                .When(x => x.ValidFrom.HasValue)
                .WithMessage("KBO LegalForm: Valid To must be greater than or equal to Valid From.");
        }
    }

    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Street)
                .NotEmpty()
                .WithMessage("Street is required.");

            RuleFor(x => x.Street)
                .Length(0, LocationListConfiguration.StreetLength)
                .WithMessage($"Street cannot be longer than {LocationListConfiguration.StreetLength}.");

            RuleFor(x => x.ZipCode)
                .NotEmpty()
                .WithMessage("Zip Code is required.");

            RuleFor(x => x.ZipCode)
                .Length(0, LocationListConfiguration.ZipCodeLength)
                .WithMessage($"Zip Code cannot be longer than {LocationListConfiguration.ZipCodeLength}.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required.");

            RuleFor(x => x.City)
                .Length(0, LocationListConfiguration.CityLength)
                .WithMessage($"City cannot be longer than {LocationListConfiguration.CityLength}.");

            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("Country is required.");

            RuleFor(x => x.Country)
                .Length(0, LocationListConfiguration.CountryLength)
                .WithMessage($"Country cannot be longer than {LocationListConfiguration.CountryLength}.");

            RuleFor(x => x.ValidTo)
                .GreaterThanOrEqualTo(x => x.ValidFrom)
                .When(x => x.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");
        }
    }

    public static class CreateOrganisationRequestMapping
    {
        public static CreateOrganisation Map(CreateOrganisationRequest message)
            => new(
                new OrganisationId(message.Id),
                message.Name,
                message.OvoNumber,
                message.ShortName,
                Article.Parse(message.Article),
                message.ParentOrganisationId.HasValue ? new OrganisationId(message.ParentOrganisationId.Value) : null,
                message.Description,
                message.PurposeIds?.Select(x => new PurposeId(x)).ToList(),
                message.ShowOnVlaamseOverheidSites,
                new ValidFrom(message.ValidFrom),
                new ValidTo(message.ValidTo),
                new ValidFrom(message.OperationalValidFrom),
                new ValidTo(message.OperationalValidTo));

        public static CreateOrganisationFromKbo MapToCreateKboOrganisation(CreateOrganisationRequest message, string kboNumber)
            => new(
                new OrganisationId(message.Id),
                message.Name,
                message.OvoNumber,
                message.ShortName,
                Article.Parse(message.Article),
                message.ParentOrganisationId.HasValue ? new OrganisationId(message.ParentOrganisationId.Value) : null,
                message.Description,
                message.PurposeIds?.Select(x => new PurposeId(x)).ToList(),
                message.ShowOnVlaamseOverheidSites,
                new ValidFrom(message.ValidFrom),
                new ValidTo(message.ValidTo),
                new KboNumber(kboNumber),
                new ValidFrom(message.OperationalValidFrom),
                new ValidTo(message.OperationalValidTo));
    }
}
