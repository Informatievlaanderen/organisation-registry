namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationSubTheme.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.RegulationSubTheme;
using OrganisationRegistry.RegulationSubTheme.Commands;
using OrganisationRegistry.RegulationTheme;
using SqlServer.RegulationSubTheme;

public class CreateRegulationSubThemeRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public Guid RegulationThemeId { get; set; }
}

public class CreateRegulationSubThemeRequestValidator : AbstractValidator<CreateRegulationSubThemeRequest>
{
    public CreateRegulationSubThemeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Name)
            .Length(0, RegulationSubThemeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {RegulationSubThemeListConfiguration.NameLength}.");

        RuleFor(x => x.RegulationThemeId)
            .NotEmpty()
            .WithMessage("RegulationThemeId is required.");
    }
}

public static class CreateRegulationSubThemeRequestMapping
{
    public static CreateRegulationSubTheme Map(CreateRegulationSubThemeRequest message)
        => new(
            new RegulationSubThemeId(message.Id),
            new RegulationSubThemeName(message.Name),
            new RegulationThemeId(message.RegulationThemeId));
}
