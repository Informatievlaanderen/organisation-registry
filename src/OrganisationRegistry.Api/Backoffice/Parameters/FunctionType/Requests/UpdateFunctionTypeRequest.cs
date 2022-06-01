namespace OrganisationRegistry.Api.Backoffice.Parameters.FunctionType.Requests;

using System;
using FluentValidation;
using Function;
using Function.Commands;
using SqlServer.FunctionType;

public class UpdateFunctionTypeInternalRequest
{
    public Guid FunctionId { get; set; }
    public UpdateFunctionTypeRequest Body { get; set; }

    public UpdateFunctionTypeInternalRequest(Guid functionTypeId, UpdateFunctionTypeRequest body)
    {
        FunctionId = functionTypeId;
        Body = body;
    }
}

public class UpdateFunctionTypeRequest
{
    public string Name { get; set; } = null!;
}

public class UpdateFunctionTypeRequestValidator : AbstractValidator<UpdateFunctionTypeInternalRequest>
{
    public UpdateFunctionTypeRequestValidator()
    {
        RuleFor(x => x.FunctionId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Body.Name)
            .Length(0, FunctionTypeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {FunctionTypeListConfiguration.NameLength}.");
    }
}

public static class UpdateFunctionTypeRequestMapping
{
    public static UpdateFunctionType Map(UpdateFunctionTypeInternalRequest message)
        => new(
            new FunctionTypeId(message.FunctionId),
            message.Body.Name);
}
