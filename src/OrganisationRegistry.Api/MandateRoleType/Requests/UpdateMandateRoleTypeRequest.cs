namespace OrganisationRegistry.Api.MandateRoleType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.MandateRoleType.Commands;
    using SqlServer.MandateRoleType;
    using OrganisationRegistry.MandateRoleType;

    public class UpdateMandateRoleTypeInternalRequest
    {
        public Guid MandateRoleTypeId { get; set; }
        public UpdateMandateRoleTypeRequest Body { get; set; }

        public UpdateMandateRoleTypeInternalRequest(Guid mandateRoleTypeId, UpdateMandateRoleTypeRequest body)
        {
            MandateRoleTypeId = mandateRoleTypeId;
            Body = body;
        }
    }

    public class UpdateMandateRoleTypeRequest
    {
        public string Name { get; set; }
    }

    public class UpdateMandateRoleTypeRequestValidator : AbstractValidator<UpdateMandateRoleTypeInternalRequest>
    {
        public UpdateMandateRoleTypeRequestValidator()
        {
            RuleFor(x => x.MandateRoleTypeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, MandateRoleTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {MandateRoleTypeListConfiguration.NameLength}.");
        }
    }

    public static class UpdateMandateRoleTypeRequestMapping
    {
        public static UpdateMandateRoleType Map(UpdateMandateRoleTypeInternalRequest message)
        {
            return new UpdateMandateRoleType(
                new MandateRoleTypeId(message.MandateRoleTypeId),
                message.Body.Name);
        }
    }
}
