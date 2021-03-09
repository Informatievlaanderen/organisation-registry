import { Injectable } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { combineLatest } from 'rxjs/observable/combineLatest';

import { AlertBuilder, AlertService } from 'core/alert';
import { PagedEvent, PagedResult, SortOrder } from 'core/pagination';
import { BaseAlertMessages } from 'core/alertmessages';
import { OidcService, Role } from 'core/auth';

import { 
  Organisation, 
  OrganisationChild, 
  OrganisationService
} from 'services/organisations';

@Injectable()
export class OrganisationInfoService {
  private organisationChangedSource: Subject<Organisation>;
  private readonly organisationChanged$: Observable<Organisation>;

  private organisationChildrenChangedSource: Subject<PagedResult<OrganisationChild>>;
  private readonly organisationChildrenChanged$: Observable<PagedResult<OrganisationChild>>;

  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie');

  private isEditableChangedSource: BehaviorSubject<boolean>;
  private readonly isEditableChanged$: Observable<boolean>;

  constructor(
    private organisationService: OrganisationService,
    private oidcService: OidcService,
    private alertService: AlertService
  ) {
    this.organisationChangedSource = new Subject<Organisation>();
    this.organisationChanged$ = this.organisationChangedSource.asObservable();

    this.organisationChildrenChangedSource = new Subject<PagedResult<OrganisationChild>>();
    this.organisationChildrenChanged$ = this.organisationChildrenChangedSource.asObservable();

    this.isEditableChangedSource = new BehaviorSubject<boolean>(false);
    this.isEditableChanged$ = this.isEditableChangedSource.asObservable();

    combineLatest(this.organisationChanged$, oidcService.hasAnyOfRoles([Role.OrganisationRegistryBeheerder]))
      .subscribe(combined => {
        const isTerminated = combined[0].isTerminated;
        const isOrganisationRegistryBeheerder = combined[1];
        this.isEditableChangedSource.next(!isTerminated || isOrganisationRegistryBeheerder);
      });
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

  loadOrganisation(id: string) {
    this.organisationService.get(id)
      .subscribe(
        item => {
          if (item)
            this.organisationChangedSource.next(item);
        },
        error => this.alertLoadError(error));
  }

  loadChildren(id: string, event?: PagedEvent) {
    let childrenRequest = (event === undefined)
      ? this.organisationService.getChildren(id, this.currentSortBy, this.currentSortOrder)
      : this.organisationService.getChildren(id, event.sortBy, event.sortOrder, event.page, event.pageSize);

    childrenRequest.subscribe(
      children => {
        if (children)
          this.organisationChildrenChangedSource.next(children);
      },
      error => this.alertLoadError(error));
  }

  changePage(id: string, event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadChildren(id, event);
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
