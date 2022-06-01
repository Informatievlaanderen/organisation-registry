namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationTheme.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.RegulationTheme;
using OrganisationRegistry.RegulationTheme.Commands;
using SqlServer.RegulationTheme;

public class CreateRegulationThemeRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

public class CreateRegulationThemeRequestValidator : AbstractValidator<CreateRegulationThemeRequest>
{
    public CreateRegulationThemeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Name)
            .Length(0, RegulationThemeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {RegulationThemeListConfiguration.NameLength}.");
    }
}

public static class CreateRegulationThemeRequestMapping
{
    public static CreateRegulationTheme Map(CreateRegulationThemeRequest message)
        => new(
            new RegulationThemeId(message.Id),
            new RegulationThemeName(message.Name));
}
