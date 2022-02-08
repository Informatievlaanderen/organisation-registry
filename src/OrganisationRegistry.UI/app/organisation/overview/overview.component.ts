import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { AlertBuilder, AlertService } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { Role, OidcService } from 'core/auth';
import { FileSaverService } from 'core/file-saver';

import {
  OrganisationFilter,
  OrganisationListItem,
  OrganisationService
} from 'services/organisations';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public organisations: PagedResult<OrganisationListItem> = new PagedResult<OrganisationListItem>();
  public canCreateOrganisation: Observable<boolean>;

  private filter: OrganisationFilter = new OrganisationFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private organisationService: OrganisationService,
    private fileSaverService: FileSaverService,
    private oidcService: OidcService
  ) { }

  ngOnInit() {
    this.loadOrganisations();

    this.canCreateOrganisation = this.oidcService.roles.map(roles => {
      if (roles.indexOf(Role.OrganisationRegistryBeheerder) !== -1 || roles.indexOf(Role.Developer) !== -1) {
        return true;
      }

      // else if (roles.indexOf(Role.OrganisatieBeheerder) !== -1) {
      //   return false;
      // }

      return false;
    });
  }

  search(event: SearchEvent<OrganisationFilter>) {
    this.filter = event.fields;
    this.loadOrganisations();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadOrganisations(event);
  }

  exportCsv(event: void) {
    this.organisationService.exportCsv(this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
      csv => this.fileSaverService.saveFile(csv, 'export_organisaties'),
      error => this.alertService.setAlert(
        new AlertBuilder()
          .error(error)
          .withTitle('Organisaties kunnen niet geëxporteerd worden!')
          .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
          .build()
      ));
  }

  private loadOrganisations(event?: PagedEvent) {
    this.isLoading = true;
    let organisations = (event === undefined)
      ? this.organisationService.getOrganisations(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationService.getOrganisations(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    organisations
      .finally(() => this.isLoading = false)
      .subscribe(
        newOrganisations => this.organisations = newOrganisations,
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Organisaties kunnen niet geladen worden!')
            .withMessage('Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')
            .build()));
  }
}
