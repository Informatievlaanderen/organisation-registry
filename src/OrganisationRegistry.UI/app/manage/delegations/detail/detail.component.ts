import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { PagedResult, PagedEvent, SortOrder } from 'core/pagination';
import { SearchEvent } from 'core/search';

import {
  Delegation,
  DelegationService,
  DelegationFilter
} from 'services/delegations';

import {
  DelegationAssignment,
  DelegationAssignmentService,
  DelegationAssignmentFilter,
  DelegationAssignmentListItem
} from 'services/delegationassignments';

@Component({
  templateUrl: 'detail.template.html',
  styleUrls: ['detail.style.css']
})
export class DelegationDetailComponent implements OnInit {
  public get isBusy() {
    return this.isLoadingDelegation || this.isLoadingDelegationAssignments;
  }

  public delegation: Delegation;
  public delegationAssignments: PagedResult<DelegationAssignmentListItem> = new PagedResult<DelegationAssignmentListItem>();

  private bodyMandateId: string;
  private filter: DelegationAssignmentFilter = new DelegationAssignmentFilter();
  private currentSortBy: string = 'personName';
  private currentSortOrder: SortOrder = SortOrder.Ascending;
  private isLoadingDelegation: boolean = true;
  private isLoadingDelegationAssignments: boolean = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private delegationService: DelegationService,
    private delegationAssignmentService: DelegationAssignmentService,
    private alertService: AlertService,
  ) {
    this.delegation = new Delegation();
  }

  ngOnInit() {
    this.route.params
      .subscribe(params => {
        let bodyMandateId = params['id'];
        this.bodyMandateId = bodyMandateId;

        this.isLoadingDelegation = true;
        this.delegationService
          .get(bodyMandateId)
          .finally(() => this.isLoadingDelegation = false)
          .subscribe(
            delegation => this.delegation = delegation,
            error => this.handleError(error));

        this.loadDelegationAssignments();
      });
  }

  changePage(event: PagedEvent) {
    this.currentSortBy = event.sortBy;
    this.currentSortOrder = event.sortOrder;
    this.loadDelegationAssignments(event);
  }

  private loadDelegationAssignments(event?: PagedEvent) {
    this.isLoadingDelegationAssignments = true;

    let delegationAssignments = (event === undefined)
      ? this.delegationAssignmentService.getDelegationAssignments(this.bodyMandateId, this.filter, this.currentSortBy, this.currentSortOrder)
      : this.delegationAssignmentService.getDelegationAssignments(this.bodyMandateId, this.filter, event.sortBy, event.sortOrder, event.page, event.pageSize);

    delegationAssignments
      .finally(() => this.isLoadingDelegationAssignments = false)
      .subscribe(x => this.delegationAssignments = x,
        error => this.alertService.setAlert(
          new Alert(
            AlertType.Error,
            'Delegatie toewijzingen kunnen niet geladen worden!',
            'Er is een fout opgetreden bij het ophalen van de gegevens. Probeer het later opnieuw.')));
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Delegatie kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de delegatie. Probeer het later opnieuw.')
        .build()
    );
  }
}
