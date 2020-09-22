import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import { OrganisationSyncService, OrganisationTermination, OrganisationTerminationFilter } from 'services/organisationsync';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class TerminatedInKboOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public organisationsTerminatedInKbo: PagedResult<OrganisationTermination> = new PagedResult<OrganisationTermination>();

  private filter: OrganisationTerminationFilter = new OrganisationTerminationFilter();
  private currentSortBy: string = 'number';
  private currentSortOrder: SortOrder = SortOrder.Descending;

  constructor(
    private alertService: AlertService,
    private organisationSyncService: OrganisationSyncService) { }

  ngOnInit() {
    this.loadOrganisationsTerminatedInKbo();
  }

  search(event: SearchEvent<OrganisationTerminationFilter>) {
    this.filter = event.fields;
    this.loadOrganisationsTerminatedInKbo();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadOrganisationsTerminatedInKbo(event);
  }

  private loadOrganisationsTerminatedInKbo(event?: PagedEvent) {
    this.isLoading = true;
    let organisationsTerminatedInKbo = (event === undefined)
      ? this.organisationSyncService.list(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationSyncService.list(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    organisationsTerminatedInKbo
      .finally(() => this.isLoading = false)
      .subscribe(
        newEvents => this.organisationsTerminatedInKbo = newEvents,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'De lijst met organisaties stopgezet in de KBO kan niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
