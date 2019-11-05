namespace OrganisationRegistry.Api.SeatType.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.SeatType;
    using OrganisationRegistry.SeatType;
    using OrganisationRegistry.SeatType.Commands;

    public class CreateSeatTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? Order { get; set; }
    }

    public class CreateSeatTypeRequestValidator : AbstractValidator<CreateSeatTypeRequest>
    {
        public CreateSeatTypeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, SeatTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {SeatTypeListConfiguration.NameLength}.");
        }
    }

    public static class CreateSeatTypeRequestMapping
    {
        public static CreateSeatType Map(CreateSeatTypeRequest message)
        {
            return new CreateSeatType(
                new SeatTypeId(message.Id),
                new SeatTypeName(message.Name),
                message.Order);
        }
    }
}
