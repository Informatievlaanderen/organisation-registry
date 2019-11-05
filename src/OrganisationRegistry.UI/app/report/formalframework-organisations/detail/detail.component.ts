import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { FileSaverService } from 'core/file-saver';

import { FormalFramework, FormalFrameworkService } from 'services/formalframeworks';

import {
    FormalFrameworkOrganisationReportListItem,
    FormalFrameworkOrganisationReportService,
    FormalFrameworkOrganisationReportFilter
} from 'services/reports/formalframework-organisations';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class FormalFrameworkDetailComponent implements OnInit {
  public isLoading: boolean = true;
  public formalFrameworkId: string;
  public formalFramework: FormalFramework;
  public formalFrameworkOrganisations: PagedResult<FormalFrameworkOrganisationReportListItem> = new PagedResult<FormalFrameworkOrganisationReportListItem>();

  private filter: FormalFrameworkOrganisationReportFilter = new FormalFrameworkOrganisationReportFilter();
  private currentSortBy: string = 'organisationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private fileSaverService: FileSaverService,
    private formalFrameworkService: FormalFrameworkService,
    private formalFrameworkOrganisationReportService: FormalFrameworkOrganisationReportService
  ) {
    this.formalFramework = new FormalFramework();
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.formalFrameworkId = id;

      this.isLoading = true;
      this.formalFrameworkService
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
                    'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));

      this.loadFormalFrameworkOrganisations(id);
    });
  }

  search(event: SearchEvent<FormalFrameworkOrganisationReportFilter>) {
    this.filter = event.fields;
    this.loadFormalFrameworkOrganisations(this.formalFrameworkId);
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadFormalFrameworkOrganisations(this.formalFrameworkId, event);
  }

  exportCsv(event: void) {
    this.formalFrameworkOrganisationReportService.exportCsv(this.formalFrameworkId, this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
      csv => this.fileSaverService.saveFile(csv, 'export_toepassingsgebied'),
      error => this.alertService.setAlert(
        new AlertBuilder()
          .error(error)
          .withTitle('Organisaties per toepassingsgebied kunnen niet geëxporteerd worden!')
          .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
          .build()
      ));
  }

  exportCsvExtended(event: void) {
    this.formalFrameworkOrganisationReportService.exportCsvExtended(this.formalFrameworkId, this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
      csv => this.fileSaverService.saveFile(csv, 'export_toepassingsgebied'),
      error => this.alertService.setAlert(
        new AlertBuilder()
          .error(error)
          .withTitle('Organisaties per toepassingsgebied kunnen niet geëxporteerd worden!')
          .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
          .build()
      ));
  }

  private loadFormalFrameworkOrganisations(formalFrameworkId: string, event?: PagedEvent) {
    this.isLoading = true;
    let data = (event === undefined)
      ? this.formalFrameworkOrganisationReportService.getFormalFrameworkOrganisations(formalFrameworkId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.formalFrameworkOrganisationReportService.getFormalFrameworkOrganisations(formalFrameworkId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    data
      .finally(() => this.isLoading = false)
      .subscribe(
        formalFrameworkOrganisations => this.formalFrameworkOrganisations = formalFrameworkOrganisations,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Organisaties per toepassingsgebied kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
