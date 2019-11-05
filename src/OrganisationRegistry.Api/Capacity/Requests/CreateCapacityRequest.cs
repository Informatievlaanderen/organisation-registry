namespace OrganisationRegistry.Api.Capacity.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.Capacity;
    using OrganisationRegistry.Capacity;
    using OrganisationRegistry.Capacity.Commands;

    public class CreateCapacityRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class CreateCapacityRequestValidator : AbstractValidator<CreateCapacityRequest>
    {
        public CreateCapacityRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, CapacityListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {CapacityListConfiguration.NameLength}.");
        }
    }

    public static class CreateCapacityRequestMapping
    {
        public static CreateCapacity Map(CreateCapacityRequest message)
        {
            return new CreateCapacity(
                new CapacityId(message.Id),
                message.Name);
        }
    }
}
