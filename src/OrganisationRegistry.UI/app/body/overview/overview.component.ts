import {Component, OnDestroy, OnInit} from '@angular/core';
import {Observable} from 'rxjs/Observable';

import {AlertBuilder, AlertService} from 'core/alert';
import {PagedResult, PagedEvent, SortOrder} from 'core/pagination';
import {SearchEvent} from 'core/search';
import {Role, OidcService} from 'core/auth';
import {FileSaverService} from 'core/file-saver';

import {
  BodyFilter,
  BodyListItem,
  BodyService
} from 'services/bodies';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BodyOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public bodies: PagedResult<BodyListItem> = new PagedResult<BodyListItem>();
  public canCreateBody: Observable<boolean>;

  private filter: BodyFilter = new BodyFilter();
  private currentSortBy: string = 'name';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private alertService: AlertService,
    private bodyService: BodyService,
    private fileSaverService: FileSaverService,
    private oidcService: OidcService,
  ) {
  }

  ngOnInit() {
    this.loadBodies();

    this.canCreateBody = this.oidcService.roles.map(roles => {
      if (roles.indexOf(Role.OrganisationRegistryBeheerder) !== -1 ||
        roles.indexOf(Role.Developer) !== -1 ||
        roles.indexOf(Role.OrganisatieBeheerder) !== -1 ||
        roles.indexOf(Role.OrgaanBeheerder) !== -1) {
        return true;
      }

      return false;
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<BodyFilter>) {
    this.filter = event.fields;
    this.loadBodies();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadBodies(event);
  }

  exportCsv(event: void) {
    this.subscriptions.push(this.bodyService.exportCsv(this.filter, this.currentSortBy, this.currentSortOrder)
      .subscribe(
        csv => this.fileSaverService.saveFile(csv, 'export_organen'),
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Organen kunnen niet geëxporteerd worden!')
            .withMessage('Er is een fout opgetreden bij het exporteren van de gegevens. Probeer het later opnieuw.')
            .build()
        )));
  }

  private loadBodies(event?: PagedEvent) {
    this.isLoading = true;
    let bodies = (event === undefined)
      ? this.bodyService.getBodies(this.filter, this.currentSortBy, this.currentSortOrder)
      : this.bodyService.getBodies(this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    this.subscriptions.push(bodies
      .finally(() => this.isLoading = false)
      .subscribe(
        newBodies => this.bodies = newBodies,
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Organen kunnen niet geladen worden!')
            .withMessage('Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')
            .build())));
  }
}
