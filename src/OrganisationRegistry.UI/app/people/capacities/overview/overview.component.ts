import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';

import {
  PersonCapacityListItem,
  PersonCapacityService
} from 'services/peoplecapacities';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class PeopleCapacitiesOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public personCapacities: PagedResult<PersonCapacityListItem>;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Persoon hoedanigheden');
  private personId: string;
  private currentSortBy: string = 'organisationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private personCapacityService: PersonCapacityService,
    private alertService: AlertService
  ) {
    this.personCapacities = new PagedResult<PersonCapacityListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.personId = params['id'];
      this.loadCapacities();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadCapacities(event);
  }

  private loadCapacities(event?: PagedEvent) {
    this.isLoading = true;
    let personCapacities = (event === undefined)
      ? this.personCapacityService.getPersonCapacities(this.personId, this.currentSortBy, this.currentSortOrder)
      : this.personCapacityService.getPersonCapacities(this.personId, event.sortBy, event.sortOrder, event.page, event.pageSize);

    personCapacities
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.personCapacities = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
