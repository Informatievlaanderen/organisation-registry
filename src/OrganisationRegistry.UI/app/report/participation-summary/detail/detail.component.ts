import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { FileSaverService } from 'core/file-saver';

import {
    ParticipationSummaryReportListItem,
    ParticipationSummaryReportService,
    ParticipationSummaryReportFilter
} from 'services/reports/participation-summary';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class ParticipationSummaryDetailComponent implements OnInit {
  public isLoading: boolean = true;
  public participationSummaries: PagedResult<ParticipationSummaryReportListItem> = new PagedResult<ParticipationSummaryReportListItem>();

  private filter: ParticipationSummaryReportFilter = new ParticipationSummaryReportFilter();
  private currentSortBy: string = 'bodyName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private fileSaverService: FileSaverService,
    private participationSummaryReportService: ParticipationSummaryReportService
  ) {
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {

      this.isLoading = true;
      this.loadParticipationSummaries();
    });
  }

  search(event: SearchEvent<ParticipationSummaryReportFilter>) {
    this.filter = event.fields;
    this.loadParticipationSummaries();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadParticipationSummaries(event);
  }

  exportCsv(event: void) {
    this.participationSummaryReportService.exportCsv(this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
      csv => this.fileSaverService.saveFile(csv, 'export_participatie'),
      error => this.alertService.setAlert(
        new AlertBuilder()
          .error(error)
          .withTitle('Participaties kunnen niet geëxporteerd worden!')
          .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
          .build()
      ));
  }

  private loadParticipationSummaries(event?: PagedEvent) {
    this.isLoading = true;
    let participationSummaries = (event === undefined)
      ? this.participationSummaryReportService.getParticipationSummaries(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.participationSummaryReportService.getParticipationSummaries(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

      participationSummaries
        .finally(() => this.isLoading = false)
        .subscribe(
          result => this.participationSummaries = result,
          error => this.alertService.setAlert(
            new Alert(
              AlertType.Error,
              'Participaties kunnen niet geladen worden!',
              'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
