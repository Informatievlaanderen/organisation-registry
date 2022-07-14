namespace OrganisationRegistry.Api.Edit.Organisation.Key;

using System;
using FluentValidation;
using KeyTypes;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.SqlServer.Organisation;
using Swashbuckle.AspNetCore.Annotations;

public class UpdateOrganisationKeyRequest
{
    /// <summary>
    /// Id van het sleuteltype.
    /// </summary>
    public Guid KeyTypeId { get; set; }

    /// <summary>
    /// Waarde van de sleutel.
    /// </summary>
    public string KeyValue { get; set; } = null!;
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

public class UpdateOrganisationKeyInternalRequestValidator : AbstractValidator<UpdateOrganisationKeyRequest>
{
    public UpdateOrganisationKeyInternalRequestValidator()
    {
        RuleFor(x => x.KeyTypeId)
            .NotEmpty()
            .WithMessage("Key Type Id is required.");

        RuleFor(x => x.KeyValue)
            .NotEmpty()
            .WithMessage("Key Value is required.");

        RuleFor(x => x.KeyValue)
            .Length(0, OrganisationKeyListConfiguration.KeyValueLength)
            .WithMessage($"Key Value cannot be longer than {OrganisationKeyListConfiguration.KeyValueLength}.");

        RuleFor(x => x.ValidTo)
            .GreaterThanOrEqualTo(x => x.ValidFrom)
            .When(x => x.ValidFrom.HasValue)
            .WithMessage("Valid To must be greater than or equal to Valid From.");
    }
}

public static class UpdateOrganisationKeyRequestMapping
{
    public static UpdateOrganisationKey Map(Guid organisationId, Guid organisationKeyId, UpdateOrganisationKeyRequest message)
        => new(
            new OrganisationKeyId(organisationKeyId),
            new OrganisationId(organisationId),
            new KeyTypeId(message.KeyTypeId),
            message.KeyValue,
            new ValidFrom(message.ValidFrom),
            new ValidTo(message.ValidTo));
}
