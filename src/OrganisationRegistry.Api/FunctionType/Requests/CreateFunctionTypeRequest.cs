namespace OrganisationRegistry.Api.FunctionType.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.FunctionType;
    using OrganisationRegistry.Function;
    using OrganisationRegistry.Function.Commands;

    public class CreateFunctionTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
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
        {
            return new CreateFunctionType(
                new FunctionTypeId(message.Id),
                message.Name);
        }
    }
}
