namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing organisation classifications.
/// Access is driven entirely by the
/// <see cref="Permission.CanManageOrganisationClassifications"/> permission and
/// its (optional) restrictions, evaluated against a
/// <see cref="ClassificationTypeContext"/> and an <see cref="OrganisationContext"/>.
///
/// A holder of an unrestricted grant (e.g. AlgemeenBeheerder) always passes; a
/// restricted holder (Cjm, Regelgeving, or DecentraalBeheerder) only passes when
/// the organisation and classificationtype id match the configured rules for that
/// role.
/// </summary>
public class OrganisationClassificationTypePolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly Guid _organisationClassificationTypeId;

    public OrganisationClassificationTypePolicy(string ovoNumber, Guid organisationClassificationTypeId)
    {
        _ovoNumber = ovoNumber;
        _organisationClassificationTypeId = organisationClassificationTypeId;
    }

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            Permission.CanManageOrganisationClassifications,
            new UserContext(user),
            new OrganisationContext(_ovoNumber),
            new ClassificationTypeContext(_organisationClassificationTypeId))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op Classificatietype voor deze organisatie";
}
