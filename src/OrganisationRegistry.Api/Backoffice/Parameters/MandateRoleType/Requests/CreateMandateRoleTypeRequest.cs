namespace OrganisationRegistry.Api.Backoffice.Parameters.MandateRoleType.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.MandateRoleType;
using OrganisationRegistry.MandateRoleType.Commands;
using SqlServer.MandateRoleType;

public class CreateMandateRoleTypeRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

public class CreateMandateRoleTypeRequestValidator : AbstractValidator<CreateMandateRoleTypeRequest>
{
    public CreateMandateRoleTypeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Name)
            .Length(0, MandateRoleTypeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {MandateRoleTypeListConfiguration.NameLength}.");
    }
}

public static class CreateMandateRoleTypeRequestMapping
{
    public static CreateMandateRoleType Map(CreateMandateRoleTypeRequest message)
        => new(
            new MandateRoleTypeId(message.Id),
            new MandateRoleTypeName(message.Name));
}
