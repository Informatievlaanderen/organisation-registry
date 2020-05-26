namespace OrganisationRegistry.Api.SeatType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.SeatType.Commands;
    using SqlServer.SeatType;
    using OrganisationRegistry.SeatType;

    public class UpdateSeatTypeInternalRequest
    {
        public Guid SeatTypeId { get; set; }
        public UpdateSeatTypeRequest Body { get; set; }

        public UpdateSeatTypeInternalRequest(Guid seatTypeId, UpdateSeatTypeRequest body)
        {
            SeatTypeId = seatTypeId;
            Body = body;
        }
    }

    public class UpdateSeatTypeRequest
    {
        public string Name { get; set; }

        public int? Order { get; set; }

        public bool IsEffective { get; set; }
    }

    public class UpdateSeatTypeRequestValidator : AbstractValidator<UpdateSeatTypeInternalRequest>
    {
        public UpdateSeatTypeRequestValidator()
        {
            RuleFor(x => x.SeatTypeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.IsEffective)
                .NotEmpty()
                .WithMessage("IsEffective is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, SeatTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {SeatTypeListConfiguration.NameLength}.");
        }
    }

    public static class UpdateSeatTypeRequestMapping
    {
        public static UpdateSeatType Map(UpdateSeatTypeInternalRequest message)
            => new UpdateSeatType(
                new SeatTypeId(message.SeatTypeId),
                new SeatTypeName(message.Body.Name),
                message.Body.Order,
                message.Body.IsEffective);
    }
}
