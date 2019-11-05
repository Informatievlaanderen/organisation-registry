namespace OrganisationRegistry.Api.LocationType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.LocationType.Commands;
    using SqlServer.LocationType;
    using OrganisationRegistry.LocationType;

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
        public string Name { get; set; }
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
        {
            return new UpdateLocationType(
                new LocationTypeId(message.LocationTypeId),
                message.Body.Name);
        }
    }
}
