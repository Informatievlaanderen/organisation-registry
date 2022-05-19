namespace OrganisationRegistry.Api.Backoffice.Parameters.Purpose.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Purpose;
    using OrganisationRegistry.Purpose.Commands;
    using SqlServer.Purpose;

    public class UpdatePurposeInternalRequest
    {
        public Guid PurposeId { get; set; }
        public UpdatePurposeRequest Body { get; set; }

        public UpdatePurposeInternalRequest(Guid purposeId, UpdatePurposeRequest body)
        {
            PurposeId = purposeId;
            Body = body;
        }
    }

    public class UpdatePurposeRequest
    {
        public string Name { get; set; } = null!;
    }

    public class UpdatePurposeRequestValidator : AbstractValidator<UpdatePurposeInternalRequest>
    {
        public UpdatePurposeRequestValidator()
        {
            RuleFor(x => x.PurposeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, PurposeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {PurposeListConfiguration.NameLength}.");
        }
    }

    public static class UpdatePurposeRequestMapping
    {
        public static UpdatePurpose Map(UpdatePurposeInternalRequest message)
            => new(
                new PurposeId(message.PurposeId),
                new PurposeName(message.Body.Name));
    }
}
