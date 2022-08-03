import { AllowedOrganisationFields } from "services";
import { Role, SecurityInfo } from "core/auth";
import { Organisation } from "../organisations";

export class OrganisationAuthorization {
  public static canViewKboManagement(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    return false;
  }

  public static canViewVlimpersManagement(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    return false;
  }

  public static allowedOrganisationFields(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return AllowedOrganisationFields.None;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return AllowedOrganisationFields.All;

    if (securityInfo.hasAnyOfRoles(Role.VlimpersBeheerder)) {
      return organisation.underVlimpersManagement
        ? AllowedOrganisationFields.OnlyVlimpers
        : AllowedOrganisationFields.None;
    }

    if (organisation.isTerminated) return AllowedOrganisationFields.None;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) {
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

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles(Role.VlimpersBeheerder)
    )
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canTerminateOrganisation(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (organisation.isTerminated) return false;

    if (
      organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles(Role.VlimpersBeheerder)
    )
      return true;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    return false;
  }

  public static canCancelCouplingWithKbo(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    return (
      organisation.kboNumber &&
      securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder)
    );
  }

  public static canCoupleWithKbo(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    return (
      !organisation.kboNumber &&
      securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder)
    );
  }

  public static canAddDaughters(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles(Role.VlimpersBeheerder)
    )
      return true;

    if (organisation.isTerminated) return false;

    if (
      !organisation.underVlimpersManagement &&
      securityInfo.isOrganisatieBeheerderFor(organisation.id)
    )
      return true;

    return false;
  }

  public static canAddAndUpdateRegulations(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (securityInfo.hasAnyOfRoles(Role.RegelgevingBeheerder)) return true;

    return false;
  }

  public static canAddAndUpdateBankAccounts(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateBuildings(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateCapacities(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (securityInfo.hasAnyOfRoles(Role.RegelgevingBeheerder)) return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateClassifications(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (securityInfo.hasAnyOfRoles(Role.RegelgevingBeheerder)) return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateContacts(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateFunctions(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateLocations(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateRelations(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateOpeningHours(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddBodies(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (
      securityInfo.hasAnyOfRoles(
        Role.AlgemeenBeheerder,
        Role.CjmBeheerder,
        Role.OrgaanBeheerder
      )
    )
      return true;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateParents(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles(Role.VlimpersBeheerder)
    )
      return true;

    if (organisation.isTerminated) return false;

    if (
      !organisation.underVlimpersManagement &&
      securityInfo.isOrganisatieBeheerderFor(organisation.id)
    )
      return true;

    return false;
  }

  public static canAddAndUpdateFormalFrameworks(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      securityInfo.hasAnyOfRoles(
        Role.VlimpersBeheerder,
        Role.RegelgevingBeheerder
      )
    )
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateKeys(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles(Role.VlimpersBeheerder)
    )
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canAddAndUpdateLabels(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    if (
      organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles(Role.VlimpersBeheerder)
    )
      return true;

    if (organisation.isTerminated) return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id)) return true;

    return false;
  }

  public static canRemoveBankAccounts(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    return false;
  }

  public static canRemoveFormalFrameworks(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    return false;
  }

  public static canRemoveFunctions(
    organisation: Organisation,
    securityInfo: SecurityInfo
  ) {
    if (!securityInfo.isLoggedIn) return false;

    if (securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder, Role.CjmBeheerder))
      return true;

    return false;
  }

  static requiresOneOfRole(securityInfo: SecurityInfo, roles: Role[]): boolean {
    return securityInfo.roles.some((r) => !!roles.find((rr) => rr == r));
  }
}
