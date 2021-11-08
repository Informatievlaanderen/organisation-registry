import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  RegulationSubTheme,
  RegulationSubThemeService,
  RegulationSubThemeFilter
} from 'services/regulation-sub-themes';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class RegulationSubThemeOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public regulationSubThemes: PagedResult<RegulationSubTheme> = new PagedResult<RegulationSubTheme>();

  private filter: RegulationSubThemeFilter = new RegulationSubThemeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private regulationSubThemeService: RegulationSubThemeService) { }

  ngOnInit() {
    this.loadRegulationSubThemes();
  }

  search(event: SearchEvent<RegulationSubThemeFilter>) {
    this.filter = event.fields;
    this.loadRegulationSubThemes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadRegulationSubThemes(event);
  }

  private loadRegulationSubThemes(event?: PagedEvent) {
    this.isLoading = true;
    let regulationSubThemes = (event === undefined)
      ? this.regulationSubThemeService.getRegulationSubThemes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.regulationSubThemeService.getRegulationSubThemes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    regulationSubThemes
      .finally(() => this.isLoading = false)
      .subscribe(
        newRegulationSubThemes => this.regulationSubThemes = newRegulationSubThemes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Regelgevingsubthema\'s kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'
          )));
  }
}
