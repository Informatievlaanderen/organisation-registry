namespace OrganisationRegistry.Api.FunctionType.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.FunctionType;
    using OrganisationRegistry.Function;
    using OrganisationRegistry.Function.Commands;

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
        public string Name { get; set; }
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
        {
            return new UpdateFunctionType(
                new FunctionTypeId(message.FunctionId),
                message.Body.Name);
        }
    }
}
