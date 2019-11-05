import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';

import { AuthService } from 'core/auth';
import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import { Role } from 'core/auth';

import {
  BodyFormalFrameworkListItem,
  BodyFormalFrameworkService,
  BodyFormalFrameworkFilter
} from 'services/bodyformalframeworks';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class BodyFormalFrameworksOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public bodyFormalFrameworks: PagedResult<BodyFormalFrameworkListItem>;
  public isOrganisationRegistryBeheerder: boolean = false;
  public isOrgaanBeheerder: boolean = false;

  private filter: BodyFormalFrameworkFilter = new BodyFormalFrameworkFilter();
  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Toepassingsgebieden');
  private bodyId: string;
  private currentSortBy: string = 'formalFrameworkName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bodyFormalFrameworkService: BodyFormalFrameworkService,
    private alertService: AlertService,
  ) {
    this.bodyFormalFrameworks = new PagedResult<BodyFormalFrameworkListItem>();
  }

  ngOnInit() {
    let roles = this.route.snapshot.data['userRoles'];
    this.isOrganisationRegistryBeheerder = roles.indexOf(Role.OrganisationRegistryBeheerder) !== -1;
    this.isOrgaanBeheerder = roles.indexOf(Role.OrgaanBeheerder) !== -1;

    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.bodyId = params['id'];
      this.loadFormalFrameworks();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  search(event: SearchEvent<BodyFormalFrameworkFilter>) {
    this.filter = event.fields;
    this.loadFormalFrameworks();
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadFormalFrameworks(event);
  }

  private loadFormalFrameworks(event?: PagedEvent) {
    this.isLoading = true;
    let bodyFormalFrameworks = (event === undefined)
      ? this.bodyFormalFrameworkService.getBodyFormalFrameworks(this.bodyId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.bodyFormalFrameworkService.getBodyFormalFrameworks(this.bodyId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    bodyFormalFrameworks
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.bodyFormalFrameworks = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
