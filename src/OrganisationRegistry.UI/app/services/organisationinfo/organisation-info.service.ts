import { Injectable, OnDestroy } from "@angular/core";

import { Observable } from "rxjs/Observable";
import { Subject } from "rxjs/Subject";
import { combineLatest } from "rxjs/observable/combineLatest";

import { AlertBuilder, AlertService } from "core/alert";
import { PagedEvent, PagedResult, SortOrder } from "core/pagination";
import { BaseAlertMessages } from "core/alertmessages";
import { OidcService, Role } from "core/auth";

import {
  Organisation,
  OrganisationChild,
  OrganisationService,
} from "services/organisations";
import { Subscription } from "rxjs/Subscription";
import { AllowedOrganisationFields } from "./allowed-organisation-fields";
import { OrganisationAuthorization } from "./organisation-authorization";
import { filter, flatMap, map, mergeMap, shareReplay } from "rxjs/operators";

@Injectable()
export class OrganisationInfoService {
  private organisationChangedSource: Subject<Organisation> =
    new Subject<Organisation>();
  private readonly organisationChanged$: Observable<Organisation> =
    this.organisationChangedSource.asObservable();

  private organisationChildrenPageChangedSource: Subject<[string, PagedEvent]> =
    new Subject<[string, PagedEvent]>();
  private readonly organisationChildrenPageChanged$: Observable<
    [string, PagedEvent]
  > = this.organisationChildrenPageChangedSource.asObservable();

  private onSecurityChanged = combineLatest(
    this.organisationChanged$,
    this.oidcService.getOrUpdateValue()
  ).pipe(shareReplay(1));

  public readonly canViewKboManagementChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canViewKboManagement(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canViewVlimpersManagementChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canViewVlimpersManagement(
          organisation,
          securityInfo
        )
      )
    );

  public readonly allowedOrganisationFieldsToUpdateChanged$: Observable<AllowedOrganisationFields> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.allowedOrganisationFields(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canUpdateOrganisationChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canUpdateOrganisation(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canTerminateOrganisationChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canTerminateOrganisation(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canCancelCouplingWithKboChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canCancelCouplingWithKbo(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canCoupleWithKboChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canCoupleWithKbo(organisation, securityInfo)
      )
    );

  public readonly canAddDaughtersChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddDaughters(organisation, securityInfo)
      )
    );

  public readonly canAddAndUpdateContactsChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateContacts(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateBankAccountsChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateBankAccounts(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateFunctionsChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateFunctions(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateLocationsChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateLocations(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateClassificationsChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateClassifications(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateBuildingsChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateBuildings(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddBodiesChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddBodies(organisation, securityInfo)
      )
    );

  public readonly canAddAndUpdateRelationsChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateRegulations(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateOpeningHoursChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateOpeningHours(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateCapacitiesChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateCapacities(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateRegulationsChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateRegulations(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateParentsChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateParents(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateFormalFrameworksChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateFormalFrameworks(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateKeysChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateKeys(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canAddAndUpdateLabelsChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canAddAndUpdateLabels(
          organisation,
          securityInfo
        )
      )
    );

  private readonly organisationChildrenChanged$: Observable<
    PagedResult<OrganisationChild>
  > = this.organisationChildrenPageChanged$.pipe(
    flatMap(([id, event]) => {
      return event === undefined
        ? this.organisationService.getChildren(
            id,
            this.currentSortBy,
            this.currentSortOrder
          )
        : this.organisationService.getChildren(
            id,
            event.sortBy,
            event.sortOrder,
            event.page,
            event.pageSize
          );
    }),
    filter((children) => !!children)
  );

  public readonly canRemoveBankAccountsChanged$ = this.onSecurityChanged.pipe(
    map(([organisation, securityInfo]) =>
      OrganisationAuthorization.canRemoveBankAccounts(
        organisation,
        securityInfo
      )
    )
  );

  public readonly canRemoveFormalFrameworksChanged$ =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.canRemoveFormalFrameworks(
          organisation,
          securityInfo
        )
      )
    );

  public readonly canRemoveFunctionsChanged$ = this.onSecurityChanged.pipe(
    map(([organisation, securityInfo]) =>
      OrganisationAuthorization.canRemoveFunctions(organisation, securityInfo)
    )
  );

  public readonly canRemoveLocationsChanged$ = this.onSecurityChanged.pipe(
    map(([organisation, securityInfo]) =>
      OrganisationAuthorization.canRemoveLocations(organisation, securityInfo)
    )
  );

  public readonly canRemoveCapacitiesChanged$ = this.onSecurityChanged.pipe(
    map(([organisation, securityInfo]) =>
      OrganisationAuthorization.canRemoveCapacities(organisation, securityInfo)
    )
  );

  private currentSortBy: string = "name";
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  public requiresOneOfRole(...roles: Role[]): Observable<boolean> {
    return this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) =>
        OrganisationAuthorization.requiresOneOfRole(securityInfo, roles)
      )
    );
  }

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages(
    "Organisatie"
  );

  private readonly isEditableChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) => {
        return (
          !organisation.isTerminated ||
          securityInfo.hasAnyOfRoles(Role.AlgemeenBeheerder)
        );
      })
    );

  private readonly isLimitedByVlimpersChanged$: Observable<boolean> =
    this.onSecurityChanged.pipe(
      map(([organisation, securityInfo]) => {
        return (
          organisation.underVlimpersManagement ||
          !securityInfo.hasAnyOfRoles(
            Role.AlgemeenBeheerder,
            Role.VlimpersBeheerder
          )
        );
      })
    );
  private currentOrganisation: Organisation;

  constructor(
    private organisationService: OrganisationService,
    private oidcService: OidcService,
    private alertService: AlertService
  ) {}

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
    this.organisationService.get(id).subscribe(
      (item) => {
        if (item) {
          this.currentOrganisation = item;
          this.organisationChangedSource.next(item);
        }
      },
      (error) => this.alertLoadError(error)
    );
  }

  loadChildren(id: string, event?: PagedEvent) {
    this.organisationChildrenPageChangedSource.next([id, event]);
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
        .build()
    );
  }
}
