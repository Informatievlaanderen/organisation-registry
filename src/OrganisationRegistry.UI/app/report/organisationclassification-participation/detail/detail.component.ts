import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Constants } from 'core/constants';
import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { FileSaverService } from 'core/file-saver';

import { OrganisationClassification, OrganisationClassificationService } from 'services/organisationclassifications';

import {
    OrganisationClassificationParticipationReportListItem,
    OrganisationClassificationParticipationReportService,
    OrganisationClassificationParticipationReportFilter
} from 'services/reports/organisationclassification-participation';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class OrganisationClassificationParticipationDetailComponent implements OnInit {
  public isLoading: boolean = true;

  public classificationId: string;
  public classificationTag: string;
  public classificationLabel: string;
  public classificationTitle: string;

  public classification: OrganisationClassification;
  public classificationOrganisationParticipations: PagedResult<OrganisationClassificationParticipationReportListItem> =
    new PagedResult<OrganisationClassificationParticipationReportListItem>();

  private filter: OrganisationClassificationParticipationReportFilter = new OrganisationClassificationParticipationReportFilter();
  private currentSortBy: string = 'bodyName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private fileSaverService: FileSaverService,
    private organisationClassificationService: OrganisationClassificationService,
    private classificationOrganisationParticipationReportService: OrganisationClassificationParticipationReportService
  ) {
    this.classification = new OrganisationClassification();
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.classificationId = id;

      let tag = params['tag'];
      this.classificationTag = tag;
      this.classificationLabel = this.classificationTag === Constants.PARTICIPATION_MINISTER_TAG ? 'ministerpost' : 'beleidsdomein';

      this.isLoading = true;
      this.organisationClassificationService
        .get(id)
        .subscribe(
          item => {
            if (item) {
              this.classification = item;
              this.classificationTitle = this.classificationTag === Constants.PARTICIPATION_MINISTER_TAG
                ? `Ministerpost ${this.classification.name}`
                : `Beleidsdomein ${this.classification.name}`;
            }
          },
          error => this.alertService.setAlert(
                new Alert(
                    AlertType.Error,
                    'Participaties kunnen niet geladen worden!',
                    'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));

      this.loadOrganisationClassificationParticipations(id);
    });
  }

  search(event: SearchEvent<OrganisationClassificationParticipationReportFilter>) {
    this.filter = event.fields;
    this.loadOrganisationClassificationParticipations(this.classificationId);
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadOrganisationClassificationParticipations(this.classificationId, event);
  }

  exportCsv(event: void) {
    this.classificationOrganisationParticipationReportService.exportCsv(this.classificationId, this.filter, this.currentSortBy, this.currentSortOrder)
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

  private loadOrganisationClassificationParticipations(classificationId: string, event?: PagedEvent) {
    this.isLoading = true;

    let classificationParticipations = (event === undefined)
      ? this.classificationOrganisationParticipationReportService.getParticipationsPerOrganisationClassificationPerBody(
          classificationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.classificationOrganisationParticipationReportService.getParticipationsPerOrganisationClassificationPerBody(
          classificationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

      classificationParticipations
        .finally(() => this.isLoading = false)
        .subscribe(
          classificationOrganisationParticipations => this.classificationOrganisationParticipations = classificationOrganisationParticipations,
          error => this.alertService.setAlert(
            new Alert(
              AlertType.Error,
              'Participaties kunnen niet geladen worden!',
              'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }
}
