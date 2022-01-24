import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  Function,
  FunctionService,
  FunctionFilter
} from 'services/functions';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class FunctionOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public functions: PagedResult<Function> = new PagedResult<Function>();

  private filter: FunctionFilter = new FunctionFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private functionService: FunctionService) { }

  ngOnInit() {
    this.loadFunctions();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<FunctionFilter>) {
    this.filter = event.fields;
    this.loadFunctions();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadFunctions(event);
  }

  private loadFunctions(event?: PagedEvent) {
    this.isLoading = true;
    let functions = (event === undefined)
      ? this.functionService.getFunctions(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.functionService.getFunctions(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(functions
      .finally(() => this.isLoading = false)
      .subscribe(
        newFunctions => this.functions = newFunctions,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Functies kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
