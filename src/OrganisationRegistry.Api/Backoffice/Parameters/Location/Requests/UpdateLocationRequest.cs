namespace OrganisationRegistry.Api.Backoffice.Parameters.Location.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Location;
    using OrganisationRegistry.Location.Commands;
    using SqlServer.Location;

    public class UpdateLocationInternalRequest
    {
        public Guid LocationId { get; set; }
        public UpdateLocationRequest Body { get; set; }

        public UpdateLocationInternalRequest(Guid locationId, UpdateLocationRequest body)
        {
            LocationId = locationId;
            Body = body;
        }
    }

    public class UpdateLocationRequest
    {
        public string? CrabLocationId { get; set; }

        public string Street { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
    }

    public class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationInternalRequest>
    {
        public UpdateLocationRequestValidator()
        {
            RuleFor(x => x.LocationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Street)
                .NotEmpty()
                .WithMessage("Street is required.");

            RuleFor(x => x.Body.Street)
                .Length(0, LocationListConfiguration.StreetLength)
                .WithMessage($"Street cannot be longer than {LocationListConfiguration.StreetLength}.");

            RuleFor(x => x.Body.ZipCode)
                .NotEmpty()
                .WithMessage("Zip Code is required.");

            RuleFor(x => x.Body.ZipCode)
                .Length(0, LocationListConfiguration.ZipCodeLength)
                .WithMessage($"Zip Code cannot be longer than {LocationListConfiguration.ZipCodeLength}.");

            RuleFor(x => x.Body.City)
                .NotEmpty()
                .WithMessage("City is required.");

            RuleFor(x => x.Body.City)
                .Length(0, LocationListConfiguration.CityLength)
                .WithMessage($"City cannot be longer than {LocationListConfiguration.CityLength}.");

            RuleFor(x => x.Body.Country)
                .NotEmpty()
                .WithMessage("Country is required.");

            RuleFor(x => x.Body.Country)
                .Length(0, LocationListConfiguration.CountryLength)
                .WithMessage($"Country cannot be longer than {LocationListConfiguration.CountryLength}.");
        }
    }

    public static class UpdateLocationRequestMapping
    {
        public static UpdateLocation Map(UpdateLocationInternalRequest message)
            => new(
                new LocationId(message.LocationId),
                message.Body.CrabLocationId,
                message.Body.Street,
                message.Body.ZipCode,
                message.Body.City,
                message.Body.Country);
    }
}
