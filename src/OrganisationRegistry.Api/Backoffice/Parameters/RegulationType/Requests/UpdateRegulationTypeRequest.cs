namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.RegulationType;
    using OrganisationRegistry.RegulationType.Commands;
    using SqlServer.RegulationType;

    public class UpdateRegulationTypeInternalRequest
    {
        public Guid RegulationTypeId { get; set; }
        public UpdateRegulationTypeRequest Body { get; set; }

        public UpdateRegulationTypeInternalRequest(Guid regulationTypeId, UpdateRegulationTypeRequest body)
        {
            RegulationTypeId = regulationTypeId;
            Body = body;
        }
    }

    public class UpdateRegulationTypeRequest
    {
        public string Name { get; set; }
    }

    public class UpdateRegulationTypeRequestValidator : AbstractValidator<UpdateRegulationTypeInternalRequest>
    {
        public UpdateRegulationTypeRequestValidator()
        {
            RuleFor(x => x.RegulationTypeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, RegulationTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {RegulationTypeListConfiguration.NameLength}.");
        }
    }

    public static class UpdateRegulationTypeRequestMapping
    {
        public static UpdateRegulationType Map(UpdateRegulationTypeInternalRequest message)
        {
            return new UpdateRegulationType(
                new RegulationTypeId(message.RegulationTypeId),
                message.Body.Name);
        }
    }
}
