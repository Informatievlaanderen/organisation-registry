import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  MandateRoleType,
  MandateRoleTypeService,
  MandateRoleTypeFilter
} from 'services/mandateroletypes';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class MandateRoleTypeOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public mandateRoleTypes: PagedResult<MandateRoleType> = new PagedResult<MandateRoleType>();

  private filter: MandateRoleTypeFilter = new MandateRoleTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private mandateRoleTypeService: MandateRoleTypeService) { }

  ngOnInit() {
    this.loadMandateRoleTypes();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<MandateRoleTypeFilter>) {
    this.filter = event.fields;
    this.loadMandateRoleTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadMandateRoleTypes(event);
  }

  private loadMandateRoleTypes(event?: PagedEvent) {
    this.isLoading = true;
    let mandateRoleTypes = (event === undefined)
      ? this.mandateRoleTypeService.getMandateRoleTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.mandateRoleTypeService.getMandateRoleTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(mandateRoleTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newMandateRoleTypes => this.mandateRoleTypes = newMandateRoleTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Mandaat rollen kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
