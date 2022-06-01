namespace OrganisationRegistry.Api.Backoffice.Parameters.SeatType.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.SeatType;
using OrganisationRegistry.SeatType.Commands;
using SqlServer.SeatType;

public class CreateSeatTypeRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Order { get; set; }

    public bool IsEffective { get; set; }
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
        => new(
            new SeatTypeId(message.Id),
            new SeatTypeName(message.Name),
            message.Order,
            message.IsEffective);
}
