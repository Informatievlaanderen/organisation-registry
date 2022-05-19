namespace OrganisationRegistry.Api.Backoffice.Parameters.Location.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Location;
    using OrganisationRegistry.Location.Commands;
    using SqlServer.Location;

    public class CreateLocationRequest
    {
        public Guid Id { get; set; }

        public string CrabLocationId { get; set; }

        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
    }

    public class CreateLocationRequestValidator : AbstractValidator<CreateLocationRequest>
    {
        public CreateLocationRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

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
        }
    }

    public static class CreateLocationRequestMapping
    {
        public static CreateLocation Map(CreateLocationRequest message)
            => new(
                new LocationId(message.Id),
                message.CrabLocationId,
                message.Street,
                message.ZipCode,
                message.City,
                message.Country);
    }
}
