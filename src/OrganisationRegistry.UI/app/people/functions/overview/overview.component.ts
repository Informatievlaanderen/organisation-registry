import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';

import { Subscription } from 'rxjs/Subscription';

import { AlertBuilder, AlertService } from 'core/alert';
import { BaseAlertMessages } from 'core/alertmessages';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';

import {
  PersonFunctionListItem,
  PersonFunctionService
} from 'services/peoplefunctions';

@Component({
  templateUrl: 'overview.template.html',
  styleUrls: ['overview.style.css']
})
export class PeopleFunctionsOverviewComponent implements OnInit, OnDestroy {
  public isLoading: boolean = true;
  public personFunctions: PagedResult<PersonFunctionListItem>;

  private readonly alertMessages: BaseAlertMessages = new BaseAlertMessages('Persoon functies');
  private personId: string;
  private currentSortBy: string = 'organisationName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private personFunctionService: PersonFunctionService,
    private alertService: AlertService
  ) {
    this.personFunctions = new PagedResult<PersonFunctionListItem>();
  }

  ngOnInit() {
    this.subscriptions.push(this.route.parent.parent.params.subscribe((params: Params) => {
      this.personId = params['id'];
      this.loadFunctions();
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadFunctions(event);
  }

  private loadFunctions(event?: PagedEvent) {
    this.isLoading = true;
    let personFunctions = (event === undefined)
      ? this.personFunctionService.getPersonFunctions(this.personId, this.currentSortBy, this.currentSortOrder)
      : this.personFunctionService.getPersonFunctions(this.personId, event.sortBy, event.sortOrder, event.page, event.pageSize);

    personFunctions
      .finally(() => this.isLoading = false)
      .subscribe(
        items => {
          if (items)
            this.personFunctions = items;
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle(this.alertMessages.loadError.title)
            .withMessage(this.alertMessages.loadError.message)
            .build()));
  }
}
