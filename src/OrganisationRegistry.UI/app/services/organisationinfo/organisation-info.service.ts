import {Injectable, OnDestroy} from '@angular/core';

import {Observable} from 'rxjs/Observable';
import {Subject} from 'rxjs/Subject';
import {BehaviorSubject} from 'rxjs/BehaviorSubject';
import {combineLatest} from 'rxjs/observable/combineLatest';

import {AlertBuilder, AlertService} from 'core/alert';
import {PagedEvent, PagedResult, SortOrder} from 'core/pagination';
import {BaseAlertMessages} from 'core/alertmessages';
import {OidcService, Role} from 'core/auth';

import {Organisation, OrganisationChild, OrganisationService} from 'services/organisations';
import {Subscription} from "rxjs/Subscription";

@Injectable()
export class OrganisationInfoService implements OnDestroy {
  private organisationChangedSource: Subject<Organisation>;
  private readonly organisationChanged$: Observable<Organisation>;

  private organisationChildrenPageChangedSource: Subject<[string, PagedEvent]>;
  private readonly organisationChildrenPageChanged$: Observable<[string, PagedEvent]>;

  private canViewKboManagementChangedSource: BehaviorSubject<boolean>;
  public readonly canViewKboManagementChanged$: Observable<boolean>;

  private canViewVlimpersManagementChangedSource: BehaviorSubject<boolean>;
  public readonly canViewVlimpersManagementChanged$: Observable<boolean>;

  private canAddAndUpdateAllOrganisationFieldsChangedSource: BehaviorSubject<boolean>;
  public readonly canAddAndUpdateAllOrganisationFieldsChanged$: Observable<boolean>;

  private canAddAndUpdateOrganisationChangedSource: BehaviorSubject<boolean>;
  public readonly canAddAndUpdateOrganisationChanged$: Observable<boolean>;

  private canTerminateOrganisationChangedSource: BehaviorSubject<boolean>;
  public readonly canTerminateOrganisationChanged$: Observable<boolean>;

  private canCancelCouplingWithKboChangedSource: BehaviorSubject<boolean>;
  public readonly canCancelCouplingWithKboChanged$: Observable<boolean>;

  private canCoupleWithKboChangedSource: BehaviorSubject<boolean>;
  public readonly canCoupleWithKboChanged$: Observable<boolean>;

  private canAddDaughtersChangedSource: BehaviorSubject<boolean>;
  public readonly canAddDaughtersChanged$: Observable<boolean>;

  private canAddAndUpdateRegulationsChangedSource: BehaviorSubject<boolean>;
  public readonly canAddAndUpdateRegulationsChanged$: Observable<boolean>;

  private canAddAndUpdateParentsChangedSource: BehaviorSubject<boolean>;
  public readonly canAddAndUpdateParentsChanged$: Observable<boolean>;

  private organisationIdChangedSource: Subject<string>;
  private readonly organisationIdChanged$: Observable<string>;

  private organisationChildrenChangedSource: Subject<PagedResult<OrganisationChild>>;
  private readonly organisationChildrenChanged$: Observable<PagedResult<OrganisationChild>>;

  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie');

  private isEditableChangedSource: BehaviorSubject<boolean>;
  private readonly isEditableChanged$: Observable<boolean>;

  private isLimitedByVlimpersChangedSource: BehaviorSubject<boolean>;
  private readonly isLimitedByVlimpersChanged$: Observable<boolean>;
  private currentOrganisation: Organisation;
  private subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private organisationService: OrganisationService,
    private oidcService: OidcService,
    private alertService: AlertService
  ) {

    this.organisationIdChangedSource = new Subject<string>();
    this.organisationIdChanged$ = this.organisationIdChangedSource.asObservable();

    this.organisationChildrenPageChangedSource = new Subject<[string, PagedEvent]>();
    this.organisationChildrenPageChanged$ = this.organisationChildrenPageChangedSource.asObservable();

    this.organisationChangedSource = new Subject<Organisation>();
    this.organisationChanged$ = this.organisationChangedSource.asObservable();

    this.organisationChildrenChangedSource = new Subject<PagedResult<OrganisationChild>>();
    this.organisationChildrenChanged$ = this.organisationChildrenChangedSource.asObservable();

    this.isEditableChangedSource = new BehaviorSubject<boolean>(false);
    this.isEditableChanged$ = this.isEditableChangedSource.asObservable();

    this.canViewKboManagementChangedSource = new BehaviorSubject<boolean>(false);
    this.canViewKboManagementChanged$ = this.canViewKboManagementChangedSource.asObservable();

    this.canViewVlimpersManagementChangedSource = new BehaviorSubject<boolean>(false);
    this.canViewVlimpersManagementChanged$ = this.canViewVlimpersManagementChangedSource.asObservable();

    this.canAddAndUpdateAllOrganisationFieldsChangedSource = new BehaviorSubject<boolean>(false);
    this.canAddAndUpdateAllOrganisationFieldsChanged$ = this.canAddAndUpdateAllOrganisationFieldsChangedSource.asObservable();

    this.canAddAndUpdateOrganisationChangedSource = new BehaviorSubject<boolean>(false);
    this.canAddAndUpdateOrganisationChanged$ = this.canAddAndUpdateOrganisationChangedSource.asObservable();

    this.canTerminateOrganisationChangedSource = new BehaviorSubject<boolean>(false);
    this.canTerminateOrganisationChanged$ = this.canTerminateOrganisationChangedSource.asObservable();

    this.canCancelCouplingWithKboChangedSource = new BehaviorSubject<boolean>(false);
    this.canCancelCouplingWithKboChanged$ = this.canCancelCouplingWithKboChangedSource.asObservable();

    this.canCoupleWithKboChangedSource = new BehaviorSubject<boolean>(false);
    this.canCoupleWithKboChanged$ = this.canCoupleWithKboChangedSource.asObservable();

    this.canAddDaughtersChangedSource = new BehaviorSubject<boolean>(false);
    this.canAddDaughtersChanged$ = this.canAddDaughtersChangedSource.asObservable();

    this.canAddAndUpdateRegulationsChangedSource = new BehaviorSubject<boolean>(false);
    this.canAddAndUpdateRegulationsChanged$ = this.canAddAndUpdateRegulationsChangedSource.asObservable();

    this.canAddAndUpdateParentsChangedSource = new BehaviorSubject<boolean>(false);
    this.canAddAndUpdateParentsChanged$ = this.canAddAndUpdateParentsChangedSource.asObservable();

    this.isLimitedByVlimpersChangedSource = new BehaviorSubject<boolean>(false);
    this.isLimitedByVlimpersChanged$ = this.isLimitedByVlimpersChangedSource.asObservable();

    this.subscriptions.push(combineLatest(this.organisationChanged$, oidcService.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]))
      .subscribe(combined => {
        const isTerminated = combined[0].isTerminated;
        const isOrganisationRegistryBeheerder = combined[1];
        this.isEditableChangedSource.next(!isTerminated || isOrganisationRegistryBeheerder);
      }));

    this.subscriptions.push(combineLatest(this.organisationChanged$, oidcService.hasAnyOfRoles(
      [Role.VlimpersBeheerder, Role.OrganisationRegistryBeheerder]))
      .subscribe(combined => {
        const underVlimpersManagement = combined[0].underVlimpersManagement;
        const hasRightsToVlimpersManagedOrganisations = combined[1];
        this.isLimitedByVlimpersChangedSource.next(underVlimpersManagement && !hasRightsToVlimpersManagedOrganisations);
      }));

    this.subscriptions.push(
      this.organisationIdChanged$
        .flatMap(x => this.organisationService.get(x))
        .subscribe(
          item => {
            if (item) {
              this.currentOrganisation = item;
              this.organisationChangedSource.next(item);
            }
          },
          error => this.alertLoadError(error)
        )
    );

    this.subscriptions.push(
      this.organisationIdChanged$
        .flatMap(x => this.organisationService.get(x))
        .subscribe(
          item => {
            if (item) {
              this.currentOrganisation = item;
              this.organisationChangedSource.next(item);
            }
          },
          error => this.alertLoadError(error)
        )
    );

    this.subscriptions.push(
      this.organisationChildrenPageChanged$
        .flatMap(x => {
          let id = x[0];
          let event = x[1]
          return (event === undefined)
            ? this.organisationService.getChildren(id, this.currentSortBy, this.currentSortOrder)
            : this.organisationService.getChildren(id, event.sortBy, event.sortOrder, event.page, event.pageSize);
        })
        .subscribe(
          children => {
            if (children)
              this.organisationChildrenChangedSource.next(children);
          },
          error => this.alertLoadError(error)
        )
    );
    this.subscriptions.push(
      combineLatest(
        this.organisationChanged,
        this.oidcService.securityInfo)
        .subscribe(combined => {
          let organisation = combined[0];
          let securityInfo = combined[1];

          this.canViewKboManagementChangedSource.next(OrganisationInfoService.canViewKboManagement(organisation, securityInfo));
          this.canViewVlimpersManagementChangedSource.next(OrganisationInfoService.canViewVlimpersManagement(organisation, securityInfo));
          this.canAddAndUpdateOrganisationChangedSource.next(OrganisationInfoService.canAddAndUpdateOrganisation(organisation, securityInfo))
          this.canAddAndUpdateAllOrganisationFieldsChangedSource.next(OrganisationInfoService.canAddAndUpdateAllOrganisationFields(organisation, securityInfo));
          this.canTerminateOrganisationChangedSource.next(OrganisationInfoService.canTerminateOrganisation(organisation, securityInfo))
          this.canCancelCouplingWithKboChangedSource.next(OrganisationInfoService.canCancelCouplingWithKbo(organisation, securityInfo))
          this.canCoupleWithKboChangedSource.next(OrganisationInfoService.canCoupleWithKbo(organisation, securityInfo))
          this.canAddDaughtersChangedSource.next(OrganisationInfoService.canAddDaughters(organisation, securityInfo))
          this.canAddAndUpdateRegulationsChangedSource.next(OrganisationInfoService.canAddAndUpdateRegulations(organisation, securityInfo))
          this.canAddAndUpdateParentsChangedSource.next(OrganisationInfoService.canAddAndUpdateParents(organisation, securityInfo))
        }));
  }

  private static canViewKboManagement(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]))
      return true;

    return false;
  }

  private static canViewVlimpersManagement(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]))
      return true;

    return false;
  }

  private static canAddAndUpdateAllOrganisationFields(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (!organisation.underVlimpersManagement &&
      securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    if (organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder]))
      return true;

    return false;
  }

  private static canAddAndUpdateOrganisation(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder]))
      return true;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  private static canTerminateOrganisation(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    return !organisation.isTerminated &&
      securityInfo.hasAnyOfRoles([Role.OrganisationRegistryBeheerder])
  }

  private static canCancelCouplingWithKbo(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    return organisation.kboNumber &&
      securityInfo.hasAnyOfRoles([Role.OrganisationRegistryBeheerder])
  }

  private static canCoupleWithKbo(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    return !organisation.kboNumber &&
      securityInfo.hasAnyOfRoles([Role.OrganisationRegistryBeheerder])
  }

  private static canAddDaughters(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder]))
      return true;

    if (!organisation.underVlimpersManagement &&
      securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  private static canAddAndUpdateRegulations(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  private static canAddAndUpdateParents(organisation, securityInfo) {
    if (!securityInfo.isLoggedIn)
      return false;

    if (securityInfo.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]))
      return true;

    if (organisation.isTerminated)
      return false;

    if (organisation.underVlimpersManagement &&
      securityInfo.hasAnyOfRoles([Role.VlimpersBeheerder]))
      return true;

    if (!organisation.underVlimpersManagement &&
      securityInfo.isOrganisatieBeheerderFor(organisation.id))
      return true;

    return false;
  }

  get organisation() {
    return this.currentOrganisation;
  }

  get organisationChanged() {
    return this.organisationChanged$;
  }

  get organisationChildrenChanged() {
    return this.organisationChildrenChanged$;
  }

  get isEditableChanged() {
    return this.isEditableChanged$;
  }

  get isLimitedByVlimpers() {
    return this.isLimitedByVlimpersChanged$;
  }

  loadOrganisation(id: string) {
    this.organisationIdChangedSource.next(id);
  }

  loadChildren(id: string, event?: PagedEvent) {
    this.organisationChildrenPageChangedSource.next([id, event]);
  }

  changePage(id: string, event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadChildren(id, event);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(x => x.unsubscribe());
  }

  private alertLoadError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle(this.alertMessages.loadError.title)
        .withMessage(this.alertMessages.loadError.message)
        .build());
  }
}
