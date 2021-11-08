import { Component, OnInit } from '@angular/core';

import { AlertService, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  RegulationTheme,
  RegulationThemeService,
  RegulationThemeFilter
} from 'services/regulation-themes';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: [ 'overview.style.css' ]
})
export class RegulationThemeOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public regulationThemes: PagedResult<RegulationTheme> = new PagedResult<RegulationTheme>();

  private filter: RegulationThemeFilter = new RegulationThemeFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private regulationThemeService: RegulationThemeService) { }

  ngOnInit() {
    this.loadRegulationThemes();
  }

  search(event: SearchEvent<RegulationThemeFilter>) {
    this.filter = event.fields;
    this.loadRegulationThemes();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadRegulationThemes(event);
  }

  private loadRegulationThemes(event?: PagedEvent) {
    this.isLoading = true;
    let regulationThemes = (event === undefined)
      ? this.regulationThemeService.getRegulationThemes(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.regulationThemeService.getRegulationThemes(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    regulationThemes
      .finally(() => this.isLoading = false)
      .subscribe(
        newRegulationThemes => this.regulationThemes = newRegulationThemes,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Regelgevingsthema\'s kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
