import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  FormalFrameworkFilter,
  FormalFrameworkListItem,
  FormalFrameworkService
} from 'services/formalframeworks';
import {Subscription} from "rxjs/Subscription";

@Component({
    templateUrl: 'overview.template.html',
    styleUrls: [ 'overview.style.css' ]
  })
  export class FormalFrameworkParticipationOverviewComponent implements OnInit, OnDestroy {
    public isLoading: boolean = true;
    public formalFrameworks: PagedResult<FormalFrameworkListItem> = new PagedResult<FormalFrameworkListItem>();

    private filter: FormalFrameworkFilter = new FormalFrameworkFilter();
    private currentSortBy: string = 'name';
    private currentSortOrder: SortOrder = SortOrder.Ascending;

    private readonly subscriptions: Subscription[] = new Array<Subscription>();

    constructor(
      private alertService: AlertService,
      private formalFrameworkService: FormalFrameworkService
    ) { }

    ngOnInit() {
      this.loadFormalFrameworks();
    }

    ngOnDestroy() {
      this.subscriptions.forEach(sub => sub.unsubscribe());
    }

    search(event: SearchEvent<FormalFrameworkFilter>) {
      this.filter = event.fields;
      this.loadFormalFrameworks();
    }

    changePage(event: PagedEvent) {
      this.currentSortBy = event.sortBy;
      this.currentSortOrder = event.sortOrder;
      this.loadFormalFrameworks(event);
    }

    private loadFormalFrameworks(event?: PagedEvent) {
      this.isLoading = true;
      let formalFrameworks = (event === undefined)
        ? this.formalFrameworkService.getFormalFrameworks(this.filter, this.currentSortBy, this.currentSortOrder)
        : this.formalFrameworkService.getFormalFrameworks(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

      this.subscriptions.push(formalFrameworks
        .finally(() => this.isLoading = false)
        .subscribe(
          newFormalFrameworks => this.formalFrameworks = newFormalFrameworks,
          error => this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Toepassingsgebieden kunnen niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')
              .build())));
    }
  }
