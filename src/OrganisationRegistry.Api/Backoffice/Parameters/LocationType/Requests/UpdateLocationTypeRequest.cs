namespace OrganisationRegistry.Api.Backoffice.Parameters.LocationType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.LocationType;
    using OrganisationRegistry.LocationType.Commands;
    using SqlServer.LocationType;

    public class UpdateLocationTypeInternalRequest
    {
        public Guid LocationTypeId { get; set; }
        public UpdateLocationTypeRequest Body { get; set; }

        public UpdateLocationTypeInternalRequest(Guid locationTypeId, UpdateLocationTypeRequest body)
        {
            LocationTypeId = locationTypeId;
            Body = body;
        }
    }

    public class UpdateLocationTypeRequest
    {
        public string Name { get; set; } = null!;
    }

    public class UpdateLocationTypeRequestValidator : AbstractValidator<UpdateLocationTypeInternalRequest>
    {
        public UpdateLocationTypeRequestValidator()
        {
            RuleFor(x => x.LocationTypeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, LocationTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {LocationTypeListConfiguration.NameLength}.");
        }
    }

    public static class UpdateLocationTypeRequestMapping
    {
        public static UpdateLocationType Map(UpdateLocationTypeInternalRequest message)
            => new(
                new LocationTypeId(message.LocationTypeId),
                new LocationTypeName(message.Body.Name));
    }
}
