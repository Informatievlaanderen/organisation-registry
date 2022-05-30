namespace OrganisationRegistry.Api.Backoffice.Organisation.Key;

using System;
using FluentValidation;
using KeyTypes;
using OrganisationRegistry.Organisation;
using SqlServer.Organisation;

public class UpdateOrganisationKeyInternalRequest
{
    public Guid OrganisationId { get; set; }
    public UpdateOrganisationKeyRequest Body { get; }

    public UpdateOrganisationKeyInternalRequest(Guid organisationId, UpdateOrganisationKeyRequest message)
    {
        OrganisationId = organisationId;
        Body = message;
    }
}

public class UpdateOrganisationKeyRequest
{
    public Guid OrganisationKeyId { get; set; }
    public Guid KeyTypeId { get; set; }
    public string KeyValue { get; set; } = null!;
    public DateTime? ValidFrom { get; set; }
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
            message.Body.OrganisationKeyId,
            new OrganisationId(message.OrganisationId),
            new KeyTypeId(message.Body.KeyTypeId),
            message.Body.KeyValue,
            new ValidFrom(message.Body.ValidFrom),
            new ValidTo(message.Body.ValidTo));
    }
}