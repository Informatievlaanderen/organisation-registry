import {AllowedOrganisationFields} from "services";
import {Role} from "core/auth";

export class OrganisationAuthorization {
  public static canViewKboManagement(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    return false;
  }

  public static canViewVlimpersManagement(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    return false;
  }

  public static allowedOrganisationFields(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return AllowedOrganisationFields.None;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return AllowedOrganisationFields.All;

    if (securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder])){
      return organisation.underVlimpersManagement ?
        AllowedOrganisationFields.OnlyVlimpers :
        AllowedOrganisationFields.None;
    }

    if (organisation.isTerminated)
      return AllowedOrganisationFields.None;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
    {
      return organisation.underVlimpersManagement ?
        AllowedOrganisationFields.AllButVlimpers :
        AllowedOrganisationFields.All;
    }

    return AllowedOrganisationFields.None;
  }

  public static canUpdateOrganisation(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canTerminateOrganisation(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (organisation.isTerminated)
      return false;

    if (organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder]))
      return true;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    return false;
  }

  public static canCancelCouplingWithKbo(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    return organisation.kboNumber &&
      securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder])
  }

  public static canCoupleWithKbo(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    return !organisation.kboNumber &&
      securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder])
  }

  public static canAddDaughters(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (!organisation.underVlimpersManagement &&
      securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateRegulations(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (securityInfo.hasAnyOfRoles([Role.RegelgevingBeheerder]))
      return true;

    return false;
  }

  public static canAddAndUpdateBankAccounts(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateBuildings(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateCapacities(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (securityInfo.hasAnyOfRoles([Role.RegelgevingBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateClassifications(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (securityInfo.hasAnyOfRoles([Role.RegelgevingBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateContacts(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateFunctions(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateLocations(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateRelations(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateOpeningHours(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddBodies(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([
      Role.AlgemeenBeheerder,
      Role.OrgaanBeheerder
    ]))
      return true;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateParents(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (!organisation.underVlimpersManagement &&
      securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateFormalFrameworks(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder, Role.RegelgevingBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateKeys(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  public static canAddAndUpdateLabels(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.AlgemeenBeheerder]))
      return true;

    if (organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

}
