import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';

import {
  PersonMandateListItem,
  PersonMandateService
} from 'services/peoplemandates';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class PeopleMandatesOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public personMandates: PagedResult<PersonMandateListItem>;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Persoon functies');
  private personId: string;
  private currentSortBy: string = 'bodyName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private personMandateService: PersonMandateService,
    private alertService: AlertService
  ) {
    this.personMandates = new PagedResult<PersonMandateListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.personId = params['id'];
      this.loadMandates();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadMandates(event);
  }

  private loadMandates(event?: PagedEvent) {
    this.isLoading = true;
    let personMandates = (event === undefined)
      ? this.personMandateService.getPersonMandates(this.personId, this.currentSortBy, this.currentSortOrder)
      : this.personMandateService.getPersonMandates(this.personId, event.sortBy, event.sortOrder, event.page, event.pageSize);

    personMandates
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.personMandates = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
