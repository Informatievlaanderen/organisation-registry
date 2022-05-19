namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationTheme.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.RegulationTheme;
    using OrganisationRegistry.RegulationTheme.Commands;
    using SqlServer.RegulationTheme;

    public class UpdateRegulationThemeInternalRequest
    {
        public Guid RegulationThemeId { get; set; }
        public UpdateRegulationThemeRequest Body { get; set; }

        public UpdateRegulationThemeInternalRequest(Guid regulationThemeId, UpdateRegulationThemeRequest body)
        {
            RegulationThemeId = regulationThemeId;
            Body = body;
        }
    }

    public class UpdateRegulationThemeRequest
    {
        public string Name { get; set; } = null!;
    }

    public class UpdateRegulationThemeRequestValidator : AbstractValidator<UpdateRegulationThemeInternalRequest>
    {
        public UpdateRegulationThemeRequestValidator()
        {
            RuleFor(x => x.RegulationThemeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, RegulationThemeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {RegulationThemeListConfiguration.NameLength}.");
        }
    }

    public static class UpdateRegulationThemeRequestMapping
    {
        public static UpdateRegulationTheme Map(UpdateRegulationThemeInternalRequest message)
            => new(
                new RegulationThemeId(message.RegulationThemeId),
                new RegulationThemeName(message.Body.Name));
    }
}
