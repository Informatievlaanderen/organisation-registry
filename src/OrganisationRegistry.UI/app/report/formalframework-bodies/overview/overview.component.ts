﻿import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
    FormalFramework,
    FormalFrameworkService,
    FormalFrameworkFilter
} from 'services/formalframeworks';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class FormalFrameworkOverviewComponent implements OnInit, OnDestroy {
    public isLoading: boolean = true;
    public formalFrameworks: PagedResult<FormalFramework> = new PagedResult<FormalFramework>();

    private filter: FormalFrameworkFilter = new FormalFrameworkFilter();
    private currentSortBy: string = 'name';
    private currentSortOrder: SortOrder = SortOrder.Ascending;

    private readonly subscriptions: Subscription[] = new Array<Subscription>();

    constructor(
        private alertService: AlertService,
        private formalFrameworkService: FormalFrameworkService) { }

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
            new Alert(
              AlertType.Error,
              'Toepassingsgebieden kunnen niet geladen worden!',
              'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
    }
}
