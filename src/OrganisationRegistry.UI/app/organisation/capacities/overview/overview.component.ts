import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { OidcService } from 'core/auth';
import {Alert, AlertBuilder, AlertService, AlertType} from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  OrganisationCapacityListItem,
  OrganisationCapacityService,
  OrganisationCapacityFilter, OrganisationCapacity
} from 'services/organisationcapacities';
import {Capacity, OrganisationInfoService} from 'services';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class OrganisationCapacitiesOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public organisationCapacities: PagedResult<OrganisationCapacityListItem>;

  private filter: OrganisationCapacityFilter = new OrganisationCapacityFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Organisatie hoedanigheden');
  private organisationId: string;
  private currentSortBy: string = 'capacityName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private organisationCapacityService: OrganisationCapacityService,
    private oidcService: OidcService,
    private alertService: AlertService,
    public store: OrganisationInfoService
  ) {
    this.organisationCapacities = new PagedResult<OrganisationCapacityListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.organisationId = params['id'];
      this.loadCapacities();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<OrganisationCapacityFilter>) {
    this.filter = event.fields;
    this.loadCapacities();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadCapacities(event);
  }

  private loadCapacities(event?: PagedEvent) {
    this.isLoading = true;
    let organisationCapacities = (event === undefined)
      ? this.organisationCapacityService.getOrganisationCapacities(this.organisationId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.organisationCapacityService.getOrganisationCapacities(this.organisationId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(organisationCapacities
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.organisationCapacities = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build())));
  }

  removeCapacity(capacity: OrganisationCapacityListItem) {
    if (!confirm("Bent u zeker? Deze actie kan niet ongedaan gemaakt worden."))
      return;

    this.isLoading = true;

    this.subscriptions.push(
      this.organisationCapacityService.delete(this.organisationId, capacity).subscribe(() => {
        this.alertService.setAlert(
          new Alert(
            AlertType.Success,
            'Hoedanigheid verwijderd!',
            'Hoedanigheid werd succesvol verwijderd.'
          ));
        this.loadCapacities()
      }, error => {
        this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Hoedanigheid kon niet verwijderd worden!',
            'Er is een fout opgetreden bij het verwijderen van de gegevens. Probeer het later opnieuw.'
          ));
        this.isLoading = false;
      }));
  }
}
