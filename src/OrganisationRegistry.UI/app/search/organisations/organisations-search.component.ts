import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {Subscription} from 'rxjs/Subscription';

import {Alert, AlertBuilder, AlertService, AlertType} from '../../core/alert';
import {PagedResult, PagedEvent, SortOrder} from '../../core/pagination';

import {
  OrganisationDocument,
  OrganisationSearchService
} from '../../services/search/organisation';

@Component({
  templateUrl: 'organisations-search.template.html',
  styleUrls: ['organisations-search.style.css']
})
export class OrganisationSearchComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisations: PagedResult<OrganisationDocument> = new PagedResult<OrganisationDocument>();

  public query: string;
  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private searchService: OrganisationSearchService,
    private route: ActivatedRoute,
    private router: Router) {
  }

  ngOnInit() {
    this.subscriptions.push(this.route
      .queryParams
      .subscribe(params => {
        // Defaults to empty string if no query param provided.
        this.query = params['q'] || '';
        this.loadOrganisations();
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  changePage(event: PagedEvent) {
    this.loadOrganisations(event);
  }

  private loadOrganisations(event?: PagedEvent) {
    this.isLoading = true;
    let organisations = (event === undefined)
      ? this.searchService.search(this.query, 'ovoNumber', SortOrder.Ascending)
      : this.searchService.search(this.query, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(organisations
      .finally(() => this.isLoading = false)
      .subscribe(
        newOrganisations => this.organisations = new PagedResult<OrganisationDocument>(
          newOrganisations.data,
          newOrganisations.page,
          newOrganisations.pageSize,
          newOrganisations.totalItems,
          newOrganisations.totalPages,
          (event === undefined) ? "ovoNumber" : event.sortBy,
          newOrganisations.sortOrder),
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Organisaties kunnen niet geladen worden!')
            .withMessage('Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')
            .build()
        )));
  }
}
