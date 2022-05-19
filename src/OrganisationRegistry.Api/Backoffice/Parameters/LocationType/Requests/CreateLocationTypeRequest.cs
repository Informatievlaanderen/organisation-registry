namespace OrganisationRegistry.Api.Backoffice.Parameters.LocationType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.LocationType;
    using OrganisationRegistry.LocationType.Commands;
    using SqlServer.LocationType;

    public class CreateLocationTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
    }

    public class CreateLocationTypeRequestValidator : AbstractValidator<CreateLocationTypeRequest>
    {
        public CreateLocationTypeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, LocationTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {LocationTypeListConfiguration.NameLength}.");
        }
    }

    public static class CreateLocationTypeRequestMapping
    {
        public static CreateLocationType Map(CreateLocationTypeRequest message)
            => new(
                new LocationTypeId(message.Id),
                new LocationTypeName(message.Name));
    }
}
