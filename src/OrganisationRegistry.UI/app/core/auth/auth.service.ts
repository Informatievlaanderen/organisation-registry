import {Injectable, OnDestroy} from '@angular/core';
import {OidcService} from "./oidc.service";
import {OrganisationInfoService} from "../../services";
import {combineLatest} from "rxjs/observable/combineLatest";
import {Role} from "./role.model";
import {Subscription} from "rxjs/Subscription";
import {BehaviorSubject} from "rxjs/BehaviorSubject";
import {Observable} from "rxjs/Observable";


@Injectable()
export class AuthService implements OnDestroy{
  // private subscriptions: Subscription[] = new Array<Subscription>();
  //
  // private canEditAllOrganisationFieldsChangedSource: BehaviorSubject<boolean>;
  // private readonly canEditAllOrganisationFieldsChanged$: Observable<boolean>;
  // private canEditLimitedOrganisationFieldsChangedSource: BehaviorSubject<boolean>;
  // private readonly canEditLimitedOrganisationFieldsChanged$: Observable<boolean>;
  // private wegwijsBeheerderCheck: Observable<boolean>;
  // private vlimpersBeheerderCheck: Observable<boolean>;
  // private organisatieBeheerderCheck: Observable<boolean>;
  //
  // constructor(
  //   private oidcService: OidcService,
  //   private organisationStore: OrganisationInfoService
  // ) {
  //   this.wegwijsBeheerderCheck = this.oidcService.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]);
  //   this.vlimpersBeheerderCheck = this.oidcService.hasAnyOfRoles([Role.VlimpersBeheerder]);
  //   this.organisatieBeheerderCheck = this.oidcService.hasAnyOfRoles([Role.OrganisatieBeheerder]);
  //
  //   this.canEditAllOrganisationFieldsChangedSource = new BehaviorSubject<boolean>(false);
  //   this.canEditAllOrganisationFieldsChanged$ = this.canEditAllOrganisationFieldsChangedSource.asObservable();
  //
  //   this.canEditLimitedOrganisationFieldsChangedSource = new BehaviorSubject<boolean>(false);
  //   this.canEditLimitedOrganisationFieldsChanged$ = this.canEditLimitedOrganisationFieldsChangedSource.asObservable();
  //
  //   this.subscriptions.push(combineLatest(this.organisationStore.organisationChanged, this.oidcService.hasAnyOfRoles(
  //     [Role.VlimpersBeheerder, Role.OrganisationRegistryBeheerder]))
  //     .subscribe(combined => {
  //       const underVlimpersManagement = combined[0].underVlimpersManagement;
  //       const hasRightsToVlimpersManagedOrganisations = combined[1];
  //       let canEditAllOrganisationFields = !underVlimpersManagement || hasRightsToVlimpersManagedOrganisations;
  //
  //       this.canEditAllOrganisationFieldsChangedSource.next(canEditAllOrganisationFields);
  //     }));
  //
  //   this.subscriptions.push(
  //     combineLatest(
  //       this.organisationStore.organisationChanged,
  //       this.oidcService.securityInfo)
  //     .subscribe(combined => {
  //       let organisation = combined[0];
  //       let securityInfo = combined[1];
  //
  //       this.canEditAllOrganisationFieldsChangedSource.next(this.canEditOrganisation(organisation, securityInfo));
  //     }));
  // }
  //
  // private canEditOrganisation(organisation, securityInfo){
  //   if (this.hasAnyOfRoles(securityInfo.roles, [Role.OrganisationRegistryBeheerder]))
  //     return true;
  //
  //   if (organisation.underVlimpersManagement && this.hasAnyOfRoles(securityInfo.roles, [Role.VlimpersBeheerder]))
  //     return true;
  //
  //   if (this.hasAnyOfRoles(securityInfo.roles, [Role.OrganisatieBeheerder]) &&
  //     securityInfo.organisationIds.findIndex(x => x === organisation.id) > -1)
  //     return true;
  //
  //   return false;
  // }
  //
  // private hasAnyOfRoles(roles, desiredRoles: Array<Role>): Observable<boolean> {
  //   return roles
  //     .map(roles => {
  //       for (let userRole of roles) {
  //         if (desiredRoles.findIndex(x => x === userRole) > -1)
  //           return true;
  //       }
  //
  //       return roles.findIndex(x => x === Role.Developer) > -1;
  //     })
  //     .catch(err => {
  //       return Observable.of(false);
  //     });
  // }
  //
  // get canEditAllOrganisationFields(){
  //   return this.canEditAllOrganisationFieldsChanged$;
  // }

  ngOnDestroy() {
    // this.subscriptions.forEach(x => x.unsubscribe());
  }
}
