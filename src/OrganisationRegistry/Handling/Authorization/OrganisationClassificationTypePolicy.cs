namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Linq;
using Infrastructure.Authorization;
using Infrastructure.Configuration;
using Organisation.Exceptions;

public class OrganisationClassificationTypePolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly Guid _organisationClassificationTypeId;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public OrganisationClassificationTypePolicy(
        string ovoNumber,
        IOrganisationRegistryConfiguration configuration,
        Guid organisationClassificationTypeId)
    {
        _ovoNumber = ovoNumber;
        _configuration = configuration;
        _organisationClassificationTypeId = organisationClassificationTypeId;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInAnyOf(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        var organisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder = _configuration.Authorization.OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder;
        var organisationClassificationTypeIdsOwnedByCjm = _configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm;

        if (user.IsInAnyOf(Role.CjmBeheerder)
            && organisationClassificationTypeIdsOwnedByCjm.Contains(_organisationClassificationTypeId))
            return AuthorizationResult.Success();

        if (user.IsInAnyOf(Role.RegelgevingBeheerder)
            && organisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder.Contains(_organisationClassificationTypeId))
            return AuthorizationResult.Success();

        if (user.IsDecentraalBeheerderForOrganisation(_ovoNumber)
            && !organisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder.Contains(_organisationClassificationTypeId)
            && !organisationClassificationTypeIdsOwnedByCjm.Contains(_organisationClassificationTypeId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op Classificatietype voor deze organisatie";
}
