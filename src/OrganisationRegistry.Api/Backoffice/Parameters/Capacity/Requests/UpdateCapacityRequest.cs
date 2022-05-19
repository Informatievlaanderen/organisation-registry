namespace OrganisationRegistry.Api.Backoffice.Parameters.Capacity.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Capacity;
    using OrganisationRegistry.Capacity.Commands;
    using SqlServer.Capacity;

    public class UpdateCapacityInternalRequest
    {
        public Guid CapacityId { get; set; }
        public UpdateCapacityRequest Body { get; set; }

        public UpdateCapacityInternalRequest(Guid capacityId, UpdateCapacityRequest body)
        {
            CapacityId = capacityId;
            Body = body;
        }
    }

    public class UpdateCapacityRequest
    {
        public string Name { get; set; } = null!;
    }

    public class UpdateCapacityRequestValidator : AbstractValidator<UpdateCapacityInternalRequest>
    {
        public UpdateCapacityRequestValidator()
        {
            RuleFor(x => x.CapacityId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, CapacityListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {CapacityListConfiguration.NameLength}.");
        }
    }

    public static class UpdateCapacityRequestMapping
    {
        public static UpdateCapacity Map(UpdateCapacityInternalRequest message)
            => new(
                new CapacityId(message.CapacityId),
                message.Body.Name);
    }
}
