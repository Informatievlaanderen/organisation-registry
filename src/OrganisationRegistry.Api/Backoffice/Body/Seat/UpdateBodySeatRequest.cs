namespace OrganisationRegistry.Api.Backoffice.Body.Seat
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;
    using SeatType;
    using OrganisationRegistry.SqlServer.Body;

    public class UpdateBodySeatInternalRequest
    {
        public Guid BodyId { get; }
        public UpdateBodySeatRequest Body { get; }

        public UpdateBodySeatInternalRequest(Guid bodyId, UpdateBodySeatRequest message)
        {
            BodyId = bodyId;
            Body = message;
        }
    }

    public class UpdateBodySeatRequest
    {
        public Guid BodySeatId { get; set; }
        public string Name { get; set; } = null!;
        public bool PaidSeat { get; set; }
        public bool EntitledToVote { get; set; }
        public Guid SeatTypeId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class UpdateBodySeatInternalRequestValidator : AbstractValidator<UpdateBodySeatInternalRequest>
    {
        public UpdateBodySeatInternalRequestValidator()
        {
            RuleFor(x => x.BodyId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.BodySeatId)
                .NotEmpty()
                .WithMessage("Body Seat Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Seat Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, BodySeatListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {BodySeatListConfiguration.NameLength}.");

            RuleFor(x => x.Body.SeatTypeId)
                .NotEmpty()
                .WithMessage("Seat Type Id is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");
        }
    }

    public static class UpdateBodySeatRequestMapping
    {
        public static UpdateBodySeat Map(UpdateBodySeatInternalRequest message)
            => new(
                new BodyId(message.BodyId),
                new BodySeatId(message.Body.BodySeatId),
                message.Body.Name,
                new SeatTypeId(message.Body.SeatTypeId),
                message.Body.PaidSeat,
                message.Body.EntitledToVote,
                new Period(
                    new ValidFrom(message.Body.ValidFrom),
                    new ValidTo(message.Body.ValidTo)));
    }
}
