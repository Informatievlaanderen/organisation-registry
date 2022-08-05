import { AllowedOrganisationFields } from "services";
import {
  hasAnyOfRoles,
  isOrganisatieBeheerderFor,
  Role,
  SecurityInfo,
} from "core/auth";
import { Organisation } from "../organisations";

export class OrganisationAuthorization {
  public static canViewKboManagement(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    return hasAnyOfRoles(
      securityInfo,
      Role.AlgemeenBeheerder,
      Role.CjmBeheerder
    );
  }

  public static canViewVlimpersManagement(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    return hasAnyOfRoles(
      securityInfo,
      Role.AlgemeenBeheerder,
      Role.CjmBeheerder
    );
  }

  public static allowedOrganisationFields(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return AllowedOrganisationFields.None;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return AllowedOrganisationFields.All;

    if (hasAnyOfRoles(securityInfo, Role.VlimpersBeheerder)) {
      return organisation.underVlimpersManagement
        ? AllowedOrganisationFields.OnlyVlimpers
        : AllowedOrganisationFields.None;
    }

    if (organisation.isTerminated) return AllowedOrganisationFields.None;

    if (isOrganisatieBeheerderFor(securityInfo, organisation.id)) {
      return organisation.underVlimpersManagement
        ? AllowedOrganisationFields.AllButVlimpers
        : AllowedOrganisationFields.All;
    }

    return AllowedOrganisationFields.None;
  }

  public static canUpdateOrganisation(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      organisation.underVlimpersManagement &&
      hasAnyOfRoles(securityInfo, Role.VlimpersBeheerder)
    )
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canTerminateOrganisation(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (organisation.isTerminated) return false;

    if (
      organisation.underVlimpersManagement &&
      hasAnyOfRoles(securityInfo, Role.VlimpersBeheerder)
    )
      return true;

    return hasAnyOfRoles(
      securityInfo,
      Role.AlgemeenBeheerder,
      Role.CjmBeheerder
    );
  }

  public static canCancelCouplingWithKbo(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    return (
      organisation.kboNumber &&
      hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder)
    );
  }

  public static canCoupleWithKbo(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    return (
      !organisation.kboNumber &&
      hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder)
    );
  }

  public static canAddDaughters(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      organisation.underVlimpersManagement &&
      hasAnyOfRoles(securityInfo, Role.VlimpersBeheerder)
    )
      return true;

    if (organisation.isTerminated) return false;

    return (
      !organisation.underVlimpersManagement &&
      isOrganisatieBeheerderFor(securityInfo, organisation.id)
    );
  }

  public static canAddAndUpdateRegulations(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    return hasAnyOfRoles(securityInfo, Role.RegelgevingBeheerder);
  }

  public static canAddAndUpdateBankAccounts(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateBuildings(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateCapacities(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (hasAnyOfRoles(securityInfo, Role.RegelgevingBeheerder)) return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateClassifications(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (hasAnyOfRoles(securityInfo, Role.RegelgevingBeheerder)) return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateContacts(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateFunctions(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateLocations(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateRelations(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateOpeningHours(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddBodies(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (
      hasAnyOfRoles(
        securityInfo,
        Role.AlgemeenBeheerder,
        Role.CjmBeheerder,
        Role.OrgaanBeheerder
      )
    )
      return true;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateParents(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      organisation.underVlimpersManagement &&
      hasAnyOfRoles(securityInfo, Role.VlimpersBeheerder)
    )
      return true;

    if (organisation.isTerminated) return false;

    return (
      !organisation.underVlimpersManagement &&
      isOrganisatieBeheerderFor(securityInfo, organisation.id)
    );
  }

  public static canAddAndUpdateFormalFrameworks(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      hasAnyOfRoles(
        securityInfo,
        Role.VlimpersBeheerder,
        Role.RegelgevingBeheerder
      )
    )
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateKeys(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      organisation.underVlimpersManagement &&
      hasAnyOfRoles(securityInfo, Role.VlimpersBeheerder)
    )
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canAddAndUpdateLabels(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (hasAnyOfRoles(securityInfo, Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      organisation.underVlimpersManagement &&
      hasAnyOfRoles(securityInfo, Role.VlimpersBeheerder)
    )
      return true;

    if (organisation.isTerminated) return false;

    return isOrganisatieBeheerderFor(securityInfo, organisation.id);
  }

  public static canRemoveBankAccounts(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    return hasAnyOfRoles(
      securityInfo,
      Role.AlgemeenBeheerder,
      Role.CjmBeheerder
    );
  }

  public static canRemoveFormalFrameworks(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    return hasAnyOfRoles(
      securityInfo,
      Role.AlgemeenBeheerder,
      Role.CjmBeheerder
    );
  }

  public static canRemoveFunctions(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    return hasAnyOfRoles(
      securityInfo,
      Role.AlgemeenBeheerder,
      Role.CjmBeheerder
    );
  }

  static requiresOneOfRole(securityInfo: SecurityInfo, roles: Role[]): boolean {
    return securityInfo.roles.some((r) => !!roles.find((rr) => rr == r));
  }
}
