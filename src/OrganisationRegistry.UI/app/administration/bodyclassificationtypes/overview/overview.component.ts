import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  BodyClassificationType,
  BodyClassificationTypeService,
  BodyClassificationTypeFilter
} from 'services/bodyclassificationtypes';


@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class BodyClassificationTypeOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public bodyClassificationTypes: PagedResult<BodyClassificationType> = new PagedResult<BodyClassificationType>();

  private filter: BodyClassificationTypeFilter = new BodyClassificationTypeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private bodyClassificationTypeService: BodyClassificationTypeService) { }

  ngOnInit() {
    this.loadBodyClassificationTypes();
  }

  search(event: SearchEvent<BodyClassificationTypeFilter>) {
    this.filter = event.fields;
    this.loadBodyClassificationTypes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBodyClassificationTypes(event);
  }

  private loadBodyClassificationTypes(event?: PagedEvent) {
    this.isLoading = true;
    let bodyClassificationTypes = (event === undefined)
      ? this.bodyClassificationTypeService.getBodyClassificationTypes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.bodyClassificationTypeService.getBodyClassificationTypes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    bodyClassificationTypes
      .finally(() => this.isLoading = false)
      .subscribe(
        newBodyClassificationTypes => this.bodyClassificationTypes = newBodyClassificationTypes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Orgaan classificatietypes kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          )));
  }
}
