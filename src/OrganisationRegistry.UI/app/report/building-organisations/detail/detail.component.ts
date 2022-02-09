import {Component, OnDestroy, OnInit} from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';
import { FileSaverService } from 'core/file-saver';

import { Building, BuildingService } from 'services/buildings';

import {
    BuildingOrganisationReportListItem,
    BuildingOrganisationReportService,
    BuildingOrganisationReportFilter
} from 'services/reports/building-organisations';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: [ 'detail.style.css' ]
})
export class BuildingDetailComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public buildingId: string;
  public building: Building;
  public buildingOrganisations: PagedResult<BuildingOrganisationReportListItem> = new PagedResult<BuildingOrganisationReportListItem>();

  private filter: BuildingOrganisationReportFilter = new BuildingOrganisationReportFilter();
  private currentSortBy: string = 'organisationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private alertService: AlertService,
    private fileSaverService: FileSaverService,
    private buildingService: BuildingService,
    private buildingOrganisationReportService: BuildingOrganisationReportService
  ) {
    this.building = new Building();
  }

  ngOnInit() {
    this.route.params.forEach((params: Params) => {
      let id = params['id'];
      this.buildingId = id;

      this.isLoading = true;
      this.subscriptions.push(this.buildingService
        .get(id)
        .subscribe(
          item => {
            if (item)
              this.building = item;
          },
          error => this.alertService.setAlert(
            new Alert(
              AlertType.Error,
              'Toepassingsgebieden kunnen niet geladen worden!',
              'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));

      this.loadBuildingOrganisations(id);
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<BuildingOrganisationReportFilter>) {
    this.filter = event.fields;
    this.loadBuildingOrganisations(this.buildingId);
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBuildingOrganisations(this.buildingId, event);
  }

  exportCsv(event: void) {
    this.subscriptions.push(this.buildingOrganisationReportService.exportCsv(this.buildingId, this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
        csv => this.fileSaverService.saveFile(csv, 'export_toepassingsgebied'),
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Organisaties per toepassingsgebied kunnen niet geëxporteerd worden!')
            .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
            .build()
        )));
  }

  private loadBuildingOrganisations(buildingId: string, event?: PagedEvent) {
    this.isLoading = true;
    let data = (event === undefined)
      ? this.buildingOrganisationReportService.getBuildingOrganisations(buildingId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.buildingOrganisationReportService.getBuildingOrganisations(buildingId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(data
      .finally(() => this.isLoading = false)
      .subscribe(
        buildingOrganisations => this.buildingOrganisations = buildingOrganisations,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Organisaties per toepassingsgebied kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.'))));
  }
}
