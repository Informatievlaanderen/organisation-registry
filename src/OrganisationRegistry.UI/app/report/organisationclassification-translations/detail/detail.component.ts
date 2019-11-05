import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { FileSaverService } from 'core/file-saver';

import { OrganisationClassification, OrganisationClassificationService } from 'services/organisationclassifications';

import {
    OrganisationClassificationTranslationReportListItem,
    OrganisationClassificationTranslationReportFilter,
    OrganisationClassificationReportService
} from 'services/reports/organisationclassifications';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class OrganisationClassificationDetailComponent implements OnInit {
  public isLoading: boolean = true;
  public classificationId: string;
  public classification: OrganisationClassification;
  public classificationOrganisations: PagedResult<OrganisationClassificationTranslationReportListItem> = new PagedResult<OrganisationClassificationTranslationReportListItem>();

  private filter: OrganisationClassificationTranslationReportFilter = new OrganisationClassificationTranslationReportFilter();
  private currentSortBy: string = 'organisationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private fileSaverService: FileSaverService,
    private organisationClassificationService: OrganisationClassificationService,
    private organisationClassificationReportService: OrganisationClassificationReportService
  ) {
    this.classification = new OrganisationClassification();
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.classificationId = id;

      this.isLoading = true;
      this.organisationClassificationService
        .get(id)
        .subscribe(
          item => {
            if (item)
              this.classification = item;
          },
          error => this.alertService.setAlert(
                new Alert(
                    AlertType.Error,
                    'Vertaling entiteiten kunnen niet geladen worden!',
                    'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));

      this.loadFormalFrameworkOrganisations(id);
    });
  }

  search(event: SearchEvent<OrganisationClassificationTranslationReportFilter>) {
    this.filter = event.fields;
    this.loadFormalFrameworkOrganisations(this.classificationId);
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadFormalFrameworkOrganisations(this.classificationId, event);
  }

  exportCsv(event: void) {
    this.organisationClassificationReportService.exportCsv(this.classificationId, this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
      csv => this.fileSaverService.saveFile(csv, 'export_beleidsdomeinen'),
      error => this.alertService.setAlert(
        new AlertBuilder()
          .error(error)
          .withTitle('Vertaling entiteiten kunnen niet geëxporteerd worden!')
          .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
          .build()
      ));
  }

  private loadFormalFrameworkOrganisations(classificationId: string, event?: PagedEvent) {
    this.isLoading = true;
    let data = (event === undefined)
      ? this.organisationClassificationReportService.getClassificationOrganisations(classificationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationClassificationReportService.getClassificationOrganisations(classificationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    data
      .finally(() => this.isLoading = false)
      .subscribe(
        classificationOrganisations => this.classificationOrganisations = classificationOrganisations,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Vertaling entiteiten kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
