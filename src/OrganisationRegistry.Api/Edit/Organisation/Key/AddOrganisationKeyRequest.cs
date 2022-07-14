namespace OrganisationRegistry.Api.Edit.Organisation.Key;

using System;
using FluentValidation;
using KeyTypes;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.SqlServer.Organisation;
using Swashbuckle.AspNetCore.Annotations;

public class AddOrganisationKeyRequest
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

public class AddOrganisationKeyInternalRequestValidator : AbstractValidator<AddOrganisationKeyRequest>
{
    public AddOrganisationKeyInternalRequestValidator()
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

public static class AddOrganisationKeyRequestMapping
{
    public static AddOrganisationKey Map(Guid organisationId, AddOrganisationKeyRequest message)
        => new(
            Guid.NewGuid(),
            new OrganisationId(organisationId),
            new KeyTypeId(message.KeyTypeId),
            message.KeyValue,
            new ValidFrom(message.ValidFrom),
            new ValidTo(message.ValidTo));
}
