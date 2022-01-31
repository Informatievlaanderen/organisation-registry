import { Component, OnInit } from '@angular/core';

import { AlertBuilder, AlertService } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { FileSaverService } from 'core/file-saver';

import {
  PersonFilter,
  PersonListItem,
  PersonService
} from 'services/people';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class PersonOverviewComponent implements OnInit {
  public isLoading: boolean = true;
  public people: PagedResult<PersonListItem> = new PagedResult<PersonListItem>();

  private filter: PersonFilter = new PersonFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private alertService: AlertService,
    private personService: PersonService,
    private fileSaverService: FileSaverService,
  ) { }

  ngOnInit() {
    this.loadPeople();
  }

  search(event: SearchEvent<PersonFilter>) {
    this.filter = event.fields;
    this.loadPeople();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadPeople(event);
  }

  exportCsv(event: void) {
    this.personService.exportCsv(this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
      csv => this.fileSaverService.saveFile(csv, 'export_personen'),
      error => this.alertService.setAlert(
        new AlertBuilder()
          .error(error)
          .withTitle('Personen kunnen niet geëxporteerd worden!')
          .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
          .build()
      ));
  }

  private loadPeople(event?: PagedEvent) {
    this.isLoading = true;
    let people = (event === undefined)
      ? this.personService.getPeople(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.personService.getPeople(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    people
      .finally(() => this.isLoading = false)
      .subscribe(
        newPeople => this.people = newPeople,
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Personen kunnen niet geladen worden!')
            .withMessage('Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')
            .build()
        ));
  }
}
