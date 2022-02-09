import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationClassificationReportListItem,
  OrganisationClassificationReportFilter,
  OrganisationClassificationReportService
} from 'services/reports/organisationclassifications';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class OrganisationClassificationOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationClassifications: PagedResult<OrganisationClassificationReportListItem> = new PagedResult<OrganisationClassificationReportListItem>();

  private filter: OrganisationClassificationReportFilter = new OrganisationClassificationReportFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private organisationClassificationService: OrganisationClassificationReportService) { }

  ngOnInit() {
    this.loadOrganisationClassifications();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationClassificationReportFilter>) {
    this.filter = event.fields;
    this.loadOrganisationClassifications();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadOrganisationClassifications(event);
  }

  private loadOrganisationClassifications(event?: PagedEvent) {
    this.isLoading = true;
    let organisationClassifications = (event === undefined)
      ? this.organisationClassificationService.getPolicyDomainClassifications(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationClassificationService.getPolicyDomainClassifications(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(organisationClassifications
      .finally(() => this.isLoading = false)
      .subscribe(
        newOrganisationClassifications => this.organisationClassifications = newOrganisationClassifications,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Organisatieclassificaties kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          ))));
  }
}
