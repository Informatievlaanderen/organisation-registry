import {Component, OnDestroy, OnInit} from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  BodyClassification,
  BodyClassificationService,
  BodyClassificationFilter
} from 'services/bodyclassifications';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class BodyClassificationOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public bodyClassifications: PagedResult<BodyClassification> = new PagedResult<BodyClassification>();

  private filter: BodyClassificationFilter = new BodyClassificationFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private bodyClassificationService: BodyClassificationService) { }

  ngOnInit() {
    this.loadBodyClassifications();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<BodyClassificationFilter>) {
    this.filter = event.fields;
    this.loadBodyClassifications();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBodyClassifications(event);
  }

  private loadBodyClassifications(event?: PagedEvent) {
    this.isLoading = true;
    let bodyClassifications = (event === undefined)
      ? this.bodyClassificationService.getBodyClassifications(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.bodyClassificationService.getBodyClassifications(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(bodyClassifications
      .finally(() => this.isLoading = false)
      .subscribe(
        newBodyClassifications => this.bodyClassifications = newBodyClassifications,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Orgaanclassificaties kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          ))));
  }
}
