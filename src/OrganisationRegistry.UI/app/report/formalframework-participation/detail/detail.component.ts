import {Component, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { FileSaverService } from 'core/file-saver';

import { FormalFramework, FormalFrameworkService } from 'services/formalframeworks';

import {
    FormalFrameworkParticipationReportListItem,
    FormalFrameworkParticipationReportService,
    FormalFrameworkParticipationReportFilter
} from 'services/reports/formalframework-participation';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class FormalFrameworkParticipationDetailComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public formalFrameworkId: string;
  public formalFramework: FormalFramework;
  public formalFrameworkParticipations: PagedResult<FormalFrameworkParticipationReportListItem> = new PagedResult<FormalFrameworkParticipationReportListItem>();

  private filter: FormalFrameworkParticipationReportFilter = new FormalFrameworkParticipationReportFilter();
  private currentSortBy: string = 'bodyName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private fileSaverService: FileSaverService,
    private bodyService: FormalFrameworkService,
    private formalFrameworkParticipationReportService: FormalFrameworkParticipationReportService
  ) {
    this.formalFramework = new FormalFramework();
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.formalFrameworkId = id;

      this.isLoading = true;
      this.subscriptions.push(this.bodyService
        .get(id)
        .subscribe(
          item => {
            if (item)
              this.formalFramework = item;
          },
          error => this.alertService.setAlert(
            new Alert(
              AlertType.Error,
              'Participaties kunnen niet geladen worden!',
              'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));

      this.loadFormalFrameworkParticipations(id);
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<FormalFrameworkParticipationReportFilter>) {
    this.filter = event.fields;
    this.loadFormalFrameworkParticipations(this.formalFrameworkId);
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadFormalFrameworkParticipations(this.formalFrameworkId, event);
  }

  exportCsv(event: void) {
    this.subscriptions.push(this.formalFrameworkParticipationReportService.exportCsv(this.formalFrameworkId, this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
        csv => this.fileSaverService.saveFile(csv, 'export_participatie'),
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Participaties kunnen niet geëxporteerd worden!')
            .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
            .build()
        )));
  }

  private loadFormalFrameworkParticipations(formalFrameworkId: string, event?: PagedEvent) {
    this.isLoading = true;
    let formalFrameworkParticipations = (event === undefined)
      ? this.formalFrameworkParticipationReportService.getParticipationsPerFormalFrameworkPerBody(formalFrameworkId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.formalFrameworkParticipationReportService.getParticipationsPerFormalFrameworkPerBody(formalFrameworkId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(formalFrameworkParticipations
      .finally(() => this.isLoading = false)
      .subscribe(
        formalFrameworkParticipations => this.formalFrameworkParticipations = formalFrameworkParticipations,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Participaties kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
