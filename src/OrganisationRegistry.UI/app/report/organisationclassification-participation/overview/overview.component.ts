import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { Constants } from 'core/constants';
import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationClassificationReportListItem,
  OrganisationClassificationReportFilter,
  OrganisationClassificationReportService
} from 'services/reports/organisationclassifications';

@Component({
    templateUrl: 'overview.template.html',
    styleUrls: [ 'overview.style.css' ]
  })
  export class OrganisationClassificationParticipationOverviewComponent implements OnInit {
    public isLoading: boolean = true;

    public classificationTag: string;
    public classificationLabel: string;
    public classifications: PagedResult<OrganisationClassificationReportListItem> = new PagedResult<OrganisationClassificationReportListItem>();

    private filter: OrganisationClassificationReportFilter = new OrganisationClassificationReportFilter();
    private currentSortBy: string = 'name';
    private currentSortOrder: SortOrder = SortOrder.Ascending;

    constructor(
      private route: ActivatedRoute,
      private router: Router,
      private alertService: AlertService,
      private organisationClassificationService: OrganisationClassificationReportService
    ) { }

    ngOnInit() {
      this.route.params.forEach((params: Params) => {
        let tag = params['tag'];
        this.classificationTag = tag;
        this.classificationLabel = this.classificationTag === Constants.PARTICIPATION_MINISTER_TAG ? 'ministerpost' : 'beleidsdomein';
      });

      this.loadClassifications();
    }

    search(event: SearchEvent<OrganisationClassificationReportFilter>) {
      this.filter = event.fields;
      this.loadClassifications();
    }

    changePage(event: PagedEvent) {
      this.currentSortBy = event.sortBy;
      this.currentSortOrder = event.sortOrder;
      this.loadClassifications(event);
    }

    private loadClassifications(event?: PagedEvent) {
      this.isLoading = true;

      let classifications: Observable<PagedResult<OrganisationClassificationReportListItem>>;

      switch (this.classificationTag) {
        case Constants.PARTICIPATION_MINISTER_TAG:
          classifications = (event === undefined)
          ? this.organisationClassificationService.getResponsibleMinisterClassifications(this.filter, this.currentSortBy, this.currentSortOrder)
          : this.organisationClassificationService.getResponsibleMinisterClassifications(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);
          break;
        default:
          classifications = (event === undefined)
          ? this.organisationClassificationService.getPolicyDomainClassifications(this.filter, this.currentSortBy, this.currentSortOrder)
          : this.organisationClassificationService.getPolicyDomainClassifications(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);
          break;
      }

      classifications
        .finally(() => this.isLoading = false)
        .subscribe(
          newClassifications => this.classifications = newClassifications,
          error => this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Beleidsdomeinen kunnen niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')
              .build()));
    }
  }
