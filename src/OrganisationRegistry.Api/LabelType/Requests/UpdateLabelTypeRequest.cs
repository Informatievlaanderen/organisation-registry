namespace OrganisationRegistry.Api.LabelType.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.LabelType;
    using OrganisationRegistry.LabelType;
    using OrganisationRegistry.LabelType.Commands;

    public class UpdateLabelTypeInternalRequest
    {
        public Guid LabelTypeId { get; set; }
        public UpdateLabelTypeRequest Body { get; set; }

        public UpdateLabelTypeInternalRequest(Guid labelTypeId, UpdateLabelTypeRequest body)
        {
            LabelTypeId = labelTypeId;
            Body = body;
        }
    }

    public class UpdateLabelTypeRequest
    {
        public string Name { get; set; }
    }

    public class UpdateLabelTypeRequestValidator : AbstractValidator<UpdateLabelTypeInternalRequest>
    {
        public UpdateLabelTypeRequestValidator()
        {
            RuleFor(x => x.LabelTypeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, LabelTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {LabelTypeListConfiguration.NameLength}.");
        }
    }

    public static class UpdateLabelTypeRequestMapping
    {
        public static UpdateLabelType Map(UpdateLabelTypeInternalRequest message)
            => new UpdateLabelType(
                new LabelTypeId(message.LabelTypeId),
                new LabelTypeName(message.Body.Name));
    }
}
