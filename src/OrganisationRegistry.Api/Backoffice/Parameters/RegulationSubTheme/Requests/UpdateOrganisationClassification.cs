namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationSubTheme.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.RegulationSubTheme;
    using OrganisationRegistry.RegulationSubTheme.Commands;
    using OrganisationRegistry.RegulationTheme;
    using SqlServer.RegulationSubTheme;

    public class UpdateRegulationSubThemeInternalRequest
    {
        public Guid RegulationSubThemeId { get; set; }
        public UpdateRegulationSubThemeRequest Body { get; set; }

        public UpdateRegulationSubThemeInternalRequest(Guid regulationSubThemeId, UpdateRegulationSubThemeRequest body)
        {
            RegulationSubThemeId = regulationSubThemeId;
            Body = body;
        }
    }

    public class UpdateRegulationSubThemeRequest
    {
        public string Name { get; set; }
        public Guid RegulationThemeId { get; set; }
    }

    public class UpdateRegulationSubThemeRequestValidator : AbstractValidator<UpdateRegulationSubThemeInternalRequest>
    {
        public UpdateRegulationSubThemeRequestValidator()
        {
            RuleFor(x => x.RegulationSubThemeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, RegulationSubThemeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {RegulationSubThemeListConfiguration.NameLength}.");

            RuleFor(x => x.Body.RegulationThemeId)
                .NotEmpty()
                .WithMessage("RegulationThemeId is required.");
        }
    }

    public static class UpdateRegulationSubThemeRequestMapping
    {
        public static UpdateRegulationSubTheme Map(UpdateRegulationSubThemeInternalRequest message)
            => new UpdateRegulationSubTheme(
                new RegulationSubThemeId(message.RegulationSubThemeId),
                new RegulationSubThemeName(message.Body.Name),
                new RegulationThemeId(message.Body.RegulationThemeId));
    }
}
