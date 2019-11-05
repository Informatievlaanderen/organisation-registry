namespace OrganisationRegistry.Api.Organisation.Requests
{
    using FluentValidation;
    using SqlServer.Location;
    using SqlServer.Organisation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Configuration;
    using KeyTypes;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Purpose;

    public class CreateOrganisationRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OvoNumber { get; set; }

        public string ShortName { get; set; }

        public Guid? ParentOrganisationId { get; set; }

        public List<Guid> PurposeIds { get; set; }

        public bool ShowOnVlaamseOverheidSites { get; set; }

        public string Description { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public string Kbo { get; set; }

        public List<BankAccount> BankAccounts { get; set; }

        public List<LegalForm> LegalForms { get; set; }

        public List<Address> Addresses { get; set; }
    }

    public class BankAccount
    {
        public string Iban { get; set; }
        public string Bic { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class LegalForm
    {
        public string Code { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class Address
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Street { get; set; }
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
                .When(x => string.IsNullOrEmpty(x.Kbo));

            RuleFor(x => x.Name)
                .Length(0, OrganisationListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {OrganisationListConfiguration.NameLength}.");

            RuleFor(x => x.ValidTo)
                .GreaterThanOrEqualTo(x => x.ValidFrom)
                .When(x => x.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleFor(x => x.BankAccounts).SetCollectionValidator(new BankAccountValidator());

            RuleFor(x => x.LegalForms).SetCollectionValidator(new LegalFormValidator());

            RuleFor(x => x.Addresses).SetCollectionValidator(new AddressValidator());
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
                .WithMessage("KBO Address: Street is required.");

            RuleFor(x => x.Street)
                .Length(0, LocationListConfiguration.StreetLength)
                .WithMessage($"KBO Address: Street cannot be longer than {LocationListConfiguration.StreetLength}.");

            RuleFor(x => x.ZipCode)
                .NotEmpty()
                .WithMessage("KBO Address: Zip Code is required.");

            RuleFor(x => x.ZipCode)
                .Length(0, LocationListConfiguration.ZipCodeLength)
                .WithMessage($"KBO Address: Zip Code cannot be longer than {LocationListConfiguration.ZipCodeLength}.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("KBO Address: City is required.");

            RuleFor(x => x.City)
                .Length(0, LocationListConfiguration.CityLength)
                .WithMessage($"KBO Address: City cannot be longer than {LocationListConfiguration.CityLength}.");

            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage("KBO Address: Country is required.");

            RuleFor(x => x.Country)
                .Length(0, LocationListConfiguration.CountryLength)
                .WithMessage($"KBO Address: Country cannot be longer than {LocationListConfiguration.CountryLength}.");

            RuleFor(x => x.ValidTo)
                .GreaterThanOrEqualTo(x => x.ValidFrom)
                .When(x => x.ValidFrom.HasValue)
                .WithMessage("KBO Address: Valid To must be greater than or equal to Valid From.");
        }
    }

    public static class CreateOrganisationRequestMapping
    {
        public static CreateOrganisation Map(CreateOrganisationRequest message)
        {
            return new CreateOrganisation(
                new OrganisationId(message.Id),
                message.Name,
                message.OvoNumber,
                message.ShortName,
                message.ParentOrganisationId.HasValue ? new OrganisationId(message.ParentOrganisationId.Value) : null,
                message.Description,
                message.PurposeIds?.Select(x => new PurposeId(x)).ToList(),
                message.ShowOnVlaamseOverheidSites,
                new ValidFrom(message.ValidFrom),
                new ValidTo(message.ValidTo));
        }

        public static CreateKboOrganisation MapToCreateKboOrganisation(
            CreateOrganisationRequest message,
            ClaimsPrincipal user)
        {
            return new CreateKboOrganisation(
                new OrganisationId(message.Id),
                message.Name,
                message.OvoNumber,
                message.ShortName,
                message.ParentOrganisationId.HasValue ? new OrganisationId(message.ParentOrganisationId.Value) : null,
                message.Description,
                message.PurposeIds?.Select(x => new PurposeId(x)).ToList(),
                message.ShowOnVlaamseOverheidSites,
                new ValidFrom(message.ValidFrom),
                new ValidTo(message.ValidTo),
                user,
                new KboNumber(message.Kbo));
        }
    }
}
