import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { FileSaverService } from 'core/file-saver';

import { Capacity, CapacityService } from 'services/capacities';

import {
    CapacityPersonReportListItem,
    CapacityPersonReportService,
    CapacityPersonReportFilter
} from 'services/reports/capacity-persons';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class CapacityDetailComponent implements OnInit {
  public isLoading: boolean = true;
  public capacityId: string;
  public capacity: Capacity;
  public capacityPersons: PagedResult<CapacityPersonReportListItem> = new PagedResult<CapacityPersonReportListItem>();

  private filter: CapacityPersonReportFilter = new CapacityPersonReportFilter();
  private currentSortBy: string = 'organisationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private fileSaverService: FileSaverService,
    private capacityService: CapacityService,
    private capacityPersonReportService: CapacityPersonReportService
  ) {
    this.capacity = new Capacity();
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.capacityId = id;

      this.isLoading = true;
      this.capacityService
        .get(id)
        .subscribe(
          item => {
            if (item)
              this.capacity = item;
          },
          error => this.alertService.setAlert(
                new Alert(
                    AlertType.Error,
                    'Hoedanigheden kunnen niet geladen worden!',
                    'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));

      this.loadCapacityPersons(id);
    });
  }

  search(event: SearchEvent<CapacityPersonReportFilter>) {
    this.filter = event.fields;
    this.loadCapacityPersons(this.capacityId);
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadCapacityPersons(this.capacityId, event);
  }

  exportCsv(event: void) {
    this.capacityPersonReportService.exportCsv(this.capacityId, this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
      csv => this.fileSaverService.saveFile(csv, 'export_hoedanigheid'),
      error => this.alertService.setAlert(
        new AlertBuilder()
          .error(error)
          .withTitle('Personen per hoedanigheid kunnen niet geëxporteerd worden!')
          .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
          .build()
      ));
  }

  private loadCapacityPersons(capacityId: string, event?: PagedEvent) {
    this.isLoading = true;
    let capacityPersons = (event === undefined)
      ? this.capacityPersonReportService.getCapacityPersons(capacityId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.capacityPersonReportService.getCapacityPersons(capacityId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    capacityPersons
      .finally(() => this.isLoading = false)
      .subscribe(
        capacityPersons => this.capacityPersons = capacityPersons,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Personen per hoedanigheid kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
