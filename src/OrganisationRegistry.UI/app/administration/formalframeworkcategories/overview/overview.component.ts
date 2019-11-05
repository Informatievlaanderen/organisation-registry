import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  FormalFrameworkCategory,
  FormalFrameworkCategoryService,
  FormalFrameworkCategoryFilter
} from 'services/formalframeworkcategories';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class FormalFrameworkCategoryOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public formalFrameworkCategories: PagedResult<FormalFrameworkCategory> = new PagedResult<FormalFrameworkCategory>();

  private filter: FormalFrameworkCategoryFilter = new FormalFrameworkCategoryFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private formalFrameworkCategoryService: FormalFrameworkCategoryService) { }

  ngOnInit() {
    this.loadFormalFrameworkCategories();
  }

  search(event: SearchEvent<FormalFrameworkCategoryFilter>) {
    this.filter = event.fields;
    this.loadFormalFrameworkCategories();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadFormalFrameworkCategories(event);
  }

  private loadFormalFrameworkCategories(event?: PagedEvent) {
    this.isLoading = true;
    let formalFrameworkCategories = (event === undefined)
      ? this.formalFrameworkCategoryService.getFormalFrameworkCategories(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.formalFrameworkCategoryService.getFormalFrameworkCategories(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    formalFrameworkCategories
      .finally(() => this.isLoading = false)
      .subscribe(
        newFormalFrameworkCategories => this.formalFrameworkCategories = newFormalFrameworkCategories,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Toepassingsgebiedcategorieën kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
