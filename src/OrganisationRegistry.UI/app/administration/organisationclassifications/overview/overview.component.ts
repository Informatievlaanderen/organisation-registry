import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationClassification,
  OrganisationClassificationService,
  OrganisationClassificationFilter
} from 'services/organisationclassifications';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class OrganisationClassificationOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public organisationClassifications: PagedResult<OrganisationClassification> = new PagedResult<OrganisationClassification>();

  private filter: OrganisationClassificationFilter = new OrganisationClassificationFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private organisationClassificationService: OrganisationClassificationService) { }

  ngOnInit() {
    this.loadOrganisationClassifications();
  }

  search(event: SearchEvent<OrganisationClassificationFilter>) {
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
      ? this.organisationClassificationService.getOrganisationClassifications(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationClassificationService.getOrganisationClassifications(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    organisationClassifications
      .finally(() => this.isLoading = false)
      .subscribe(
        newOrganisationClassifications => this.organisationClassifications = newOrganisationClassifications,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Organisatieclassificaties kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          )));
  }
}
