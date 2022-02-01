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
