namespace OrganisationRegistry.Api.Backoffice.Parameters.FunctionType.Requests;

using System;
using FluentValidation;
using Function;
using Function.Commands;
using SqlServer.FunctionType;

public class CreateFunctionTypeRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

public class CreateFunctionTypeRequestValidator : AbstractValidator<CreateFunctionTypeRequest>
{
    public CreateFunctionTypeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Name)
            .Length(0, FunctionTypeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {FunctionTypeListConfiguration.NameLength}.");
    }
}

public static class CreateFunctionTypeRequestMapping
{
    public static CreateFunctionType Map(CreateFunctionTypeRequest message)
        => new(
            new FunctionTypeId(message.Id),
            message.Name);
}