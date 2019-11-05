import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { AuthService, Role } from 'core/auth';

import {
  ProjectionStateFilter,
  ProjectionStateListItem,
  ProjectionStateService
} from 'services/projectionstates';

@Component({
    templateUrl: 'overview.template.html',
    styleUrls: [ 'overview.style.css' ]
  })
  export class ProjectionStateOverviewComponent {
    public isLoading: boolean = true;
    public projectionStates: PagedResult<ProjectionStateListItem> = new PagedResult<ProjectionStateListItem>();
    public lastEventNumber: number = 0;

    private filter: ProjectionStateFilter = new ProjectionStateFilter();
    private currentSortBy: string = 'name';
    private currentSortOrder: SortOrder = SortOrder.Ascending;

    constructor(
      private alertService: AlertService,
      private projectionStateService: ProjectionStateService,
    ) { }

    ngOnInit() {
      this.loadProjectionStates();
      this.getLastEventNumber();
    }

    search(event: SearchEvent<ProjectionStateFilter>) {
      this.filter = event.fields;
      this.loadProjectionStates();
    }

    changePage(event: PagedEvent) {
      this.currentSortBy = event.sortBy;
      this.currentSortOrder = event.sortOrder;
      this.loadProjectionStates(event);
    }

    private getLastEventNumber() {
      this.isLoading = true;
      let lastEvent = this.projectionStateService.getLastEventNumber();

      lastEvent
        .finally(() => this.isLoading = false)
        .subscribe(
          data => this.lastEventNumber = data,
          error => this.alertService.setAlert(
            new Alert(
              AlertType.Error,
              'Totalen voor participaties kunnen niet geladen worden!',
              'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
    }

    private loadProjectionStates(event?: PagedEvent) {
      this.isLoading = true;
      let projectionStates = (event === undefined)
        ? this.projectionStateService.getProjectionStates(this.filter, this.currentSortBy, this.currentSortOrder)
        : this.projectionStateService.getProjectionStates(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

      projectionStates
        .finally(() => this.isLoading = false)
        .subscribe(
          newProjectionStates => this.projectionStates = newProjectionStates,
          error => this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Projectie statussen kunnen niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de projectie statussen. Probeer het later opnieuw.')
              .build()
          ));
    }
  }
