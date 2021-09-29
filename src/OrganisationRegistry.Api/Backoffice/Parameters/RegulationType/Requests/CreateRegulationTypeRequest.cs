namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.RegulationType;
    using OrganisationRegistry.RegulationType.Commands;
    using SqlServer.RegulationType;

    public class CreateRegulationTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class CreateRegulationTypeRequestValidator : AbstractValidator<CreateRegulationTypeRequest>
    {
        public CreateRegulationTypeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, RegulationTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {RegulationTypeListConfiguration.NameLength}.");
        }
    }

    public static class CreateRegulationTypeRequestMapping
    {
        public static CreateRegulationType Map(CreateRegulationTypeRequest message)
        {
            return new CreateRegulationType(
                new RegulationTypeId(message.Id),
                message.Name);
        }
    }
}
