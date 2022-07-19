import { Component, OnInit, OnDestroy } from "@angular/core";
import { ActivatedRoute, Params, Router } from "@angular/router";

import { Subscription } from "rxjs/Subscription";

import { OidcService, Role } from "core/auth";
import { AlertBuilder, AlertService } from "core/alert";
import { BaseAlertMessages } from "core/alertmessages";
import { PagedResult, PagedEvent, SortOrder } from "core/pagination";
import { SearchEvent } from "core/search";

import {
  OrganisationBankAccountListItem,
  OrganisationBankAccountService,
  OrganisationBankAccountFilter,
} from "services/organisationbankaccounts";
import { OrganisationInfoService } from "services";
import { Observable } from "rxjs/Observable";
import { map } from "rxjs/operators";
import { forkJoin } from "rxjs/observable/forkJoin";

@Component({
  templateUrl: "overview.template.html",
  styleUrls: ["overview.style.css"],
})
export class OrganisationBankAccountsOverviewComponent
  implements OnInit, OnDestroy
{
  public isLoading: boolean = true;
  public organisationBankAccounts: PagedResult<OrganisationBankAccountListItem>;
  public canRemove: Observable<boolean>;

  private filter: OrganisationBankAccountFilter =
    new OrganisationBankAccountFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages(
    "Organisatie bankrekeningnummer"
  );
  private organisationId: string;
  private currentSortBy: string = "bankAccountNumber";
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationBankAccountService: OrganisationBankAccountService,
    private oidcService: OidcService,
    private alertService: AlertService,
    public store: OrganisationInfoService
  ) {
    this.organisationBankAccounts =
      new PagedResult<OrganisationBankAccountListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(
      this.route.parent.parent.params.subscribe((params: Params) => {
        this.organisationId = params["id"];
        this.loadBankAccounts();
      })
    );

    this.canRemove = forkJoin([
      this.oidcService.isLoggedIn,
      this.oidcService.hasAnyOfRoles([Role.AlgemeenBeheerder]),
    ]).pipe(map(([a, b]) => a && b));
  }

  ngOnDestroy() {
    this.subscriptions.forEach((sub) => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationBankAccountFilter>) {
    this.filter = event.fields;
    this.loadBankAccounts();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBankAccounts(event);
  }

  private loadBankAccounts(event?: PagedEvent) {
    this.isLoading = true;
    let bankAccounts =
      event === undefined
        ? this.organisationBankAccountService.getOrganisationBankAccounts(
            this.organisationId,
            this.filter,
            this.currentSortBy,
            this.currentSortOrder
          )
        : this.organisationBankAccountService.getOrganisationBankAccounts(
            this.organisationId,
            this.filter,
            event.sortBy,
            event.sortOrder,
            event.page,
            event.pageSize
          );

    this.subscriptions.push(
      bankAccounts
        .finally(() => (this.isLoading = false))
        .subscribe(
          (items) => {
            if (items) this.organisationBankAccounts = items;
          },
          (error) =>
            this.alertService.setAlert(
              new AlertBuilder()
                .error(error)
                .withTitle(this.alertMessages.loadError.title)
                .withMessage(this.alertMessages.loadError.message)
                .build()
            )
        )
    );
  }

  removeBankAccount(bankAccount: OrganisationBankAccountListItem) {
    if (!confirm("Bent u zeker? Deze actie kan niet ongedaan gemaakt worden."))
      return;

    this.subscriptions.push(
      this.organisationBankAccountService
        .delete(this.organisationId, bankAccount.organisationBankAccountId)
        .subscribe(() => this.loadBankAccounts())
    );
  }
}
