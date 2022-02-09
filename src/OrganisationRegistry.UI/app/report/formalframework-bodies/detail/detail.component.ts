import {Component, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { FileSaverService } from 'core/file-saver';

import { FormalFramework, FormalFrameworkService } from 'services/formalframeworks';

import {
    FormalFrameworkBodyReportListItem,
    FormalFrameworkBodyReportService,
    FormalFrameworkBodyReportFilter
} from 'services/reports/formalframework-bodies';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class FormalFrameworkDetailComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public formalFrameworkId: string;
  public formalFramework: FormalFramework;
  public formalFrameworkBodies: PagedResult<FormalFrameworkBodyReportListItem> = new PagedResult<FormalFrameworkBodyReportListItem>();

  private filter: FormalFrameworkBodyReportFilter = new FormalFrameworkBodyReportFilter();
  private currentSortBy: string = 'bodyName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private fileSaverService: FileSaverService,
    private formalFrameworkService: FormalFrameworkService,
    private formalFrameworkBodyReportService: FormalFrameworkBodyReportService
  ) {
    this.formalFramework = new FormalFramework();
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.formalFrameworkId = id;

      this.isLoading = true;
      this.subscriptions.push(this.formalFrameworkService
        .get(id)
        .subscribe(
          item => {
            if (item)
              this.formalFramework = item;
          },
          error => this.alertService.setAlert(
            new Alert(
              AlertType.Error,
              'Toepassingsgebieden kunnen niet geladen worden!',
              'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));

      this.loadFormalFrameworkBodies(id);
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<FormalFrameworkBodyReportFilter>) {
    this.filter = event.fields;
    this.loadFormalFrameworkBodies(this.formalFrameworkId);
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadFormalFrameworkBodies(this.formalFrameworkId, event);
  }

  exportCsv(event: void) {
    this.subscriptions.push(this.formalFrameworkBodyReportService.exportCsv(this.formalFrameworkId, this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
        csv => this.fileSaverService.saveFile(csv, 'export_toepassingsgebied'),
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Organen per toepassingsgebied kunnen niet geëxporteerd worden!')
            .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
            .build()
        )));
  }

  private loadFormalFrameworkBodies(formalFrameworkId: string, event?: PagedEvent) {
    this.isLoading = true;
    let data = (event === undefined)
      ? this.formalFrameworkBodyReportService.getFormalFrameworkBodies(formalFrameworkId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.formalFrameworkBodyReportService.getFormalFrameworkBodies(formalFrameworkId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(data
      .finally(() => this.isLoading = false)
      .subscribe(
        formalFrameworkBodies => this.formalFrameworkBodies = formalFrameworkBodies,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Organen per toepassingsgebied kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
