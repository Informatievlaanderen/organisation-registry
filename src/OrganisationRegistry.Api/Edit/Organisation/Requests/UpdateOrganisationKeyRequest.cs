namespace OrganisationRegistry.Api.Edit.Organisation.Requests
{
    using System;
    using FluentValidation;
    using KeyTypes;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using SqlServer.Organisation;
    using Swashbuckle.AspNetCore.Annotations;

    public class UpdateOrganisationKeyInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public Guid OrganisationKeyId { get; set; }
        public UpdateOrganisationKeyRequest Body { get; }

        public UpdateOrganisationKeyInternalRequest(Guid organisationId,
            Guid organisationKeyId,
            UpdateOrganisationKeyRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
            OrganisationKeyId = organisationKeyId;
        }
    }

    public class UpdateOrganisationKeyRequest
    {
        /// <summary>
        /// Id van het sleuteltype.
        /// </summary>
        public Guid KeyTypeId { get; set; }
        /// <summary>
        /// Waarde van de sleutel.
        /// </summary>
        public string KeyValue { get; set; }
        /// <summary>
        /// Geldig vanaf.
        /// </summary>
        [SwaggerSchema(Format = "date")]
        public DateTime? ValidFrom { get; set; }
        /// <summary>
        /// Geldig tot.
        /// </summary>
        [SwaggerSchema(Format = "date")]
        public DateTime? ValidTo { get; set; }
    }

    public class UpdateOrganisationKeyInternalRequestValidator : AbstractValidator<UpdateOrganisationKeyInternalRequest>
    {
        public UpdateOrganisationKeyInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.KeyTypeId)
                .NotEmpty()
                .WithMessage("Key Type Id is required.");

            RuleFor(x => x.Body.KeyValue)
                .NotEmpty()
                .WithMessage("Key Value is required.");

            RuleFor(x => x.Body.KeyValue)
                .Length(0, OrganisationKeyListConfiguration.KeyValueLength)
                .WithMessage($"Key Value cannot be longer than {OrganisationKeyListConfiguration.KeyValueLength}.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");
        }
    }

    public static class UpdateOrganisationKeyRequestMapping
    {
        public static UpdateOrganisationKey Map(UpdateOrganisationKeyInternalRequest message)
        {
            return new UpdateOrganisationKey(
                new OrganisationKeyId(message.OrganisationKeyId),
                new OrganisationId(message.OrganisationId),
                new KeyTypeId(message.Body.KeyTypeId),
                message.Body.KeyValue,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
