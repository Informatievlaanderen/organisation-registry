namespace OrganisationRegistry.Organisation.Import;

using System;

public struct OrganisationParentIdentifier
{
    private string? Reference { get; }
    private Guid? Id { get; }

    private OrganisationParentIdentifier(Guid id)
    {
        Id = id;
        Reference = default;
        Type = IdentifierType.Id;
    }

    private OrganisationParentIdentifier(string reference)
    {
        Reference = reference;
        Id = default;
        Type = IdentifierType.Reference;
    }

    public IdentifierType Type { get; }

    public static implicit operator OrganisationParentIdentifier(Guid parentId)
        => new(parentId);

    public static implicit operator Guid(OrganisationParentIdentifier parent)
        => parent.Id ?? throw new NullReferenceException("OrganisationParentId was not provided for this parent");

    public static implicit operator OrganisationParentIdentifier(string parentReference)
        => new(parentReference);

    public static implicit operator string(OrganisationParentIdentifier parent)
        => parent.Reference?.ToLowerInvariant() ?? throw new NullReferenceException("ParentReference was not provided for this parent");

    public enum IdentifierType
    {
        Id,
        Reference,
    }

    public override string ToString()
        => Type == IdentifierType.Reference ? Reference! : Id.ToString()!;
}
