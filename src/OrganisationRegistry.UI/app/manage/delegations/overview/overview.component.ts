import { Component, OnInit } from '@angular/core';

import { Observable } from 'rxjs/Observable';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  DelegationListItem,
  DelegationService,
  DelegationFilter
} from 'services/delegations';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class DelegationOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public delegations: PagedResult<DelegationListItem> = new PagedResult<DelegationListItem>();
  public hasNonDelegatedDelegations: boolean = false;

  private filter: DelegationFilter = new DelegationFilter();
  private currentSortBy: string = 'bodyName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private delegationService: DelegationService) { }

  ngOnInit() {
    this.loadDelegations();
  }

  search(event: SearchEvent<DelegationFilter>) {
    this.filter = event.fields;
    this.loadDelegations();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadDelegations(event);
  }

  private loadDelegations(event?: PagedEvent) {
    this.isLoading = true;

    let delegations = (event === undefined)
      ? this.delegationService.getDelegations(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.delegationService.getDelegations(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    let hasNonDelegatedDelegationsCheck = this.delegationService.hasNonDelegatedDelegations(this.filter);

    Observable.zip(delegations, hasNonDelegatedDelegationsCheck)
      .map(zipped => {
        return {
          delegations: zipped[0],
          hasNonDelegatedDelegations: zipped[1]
        };
      })
      .finally(() => this.isLoading = false)
      .subscribe(
        newDelegations => {
          this.delegations = newDelegations.delegations;
          this.hasNonDelegatedDelegations = newDelegations.hasNonDelegatedDelegations;
        },
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Delegaties kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
