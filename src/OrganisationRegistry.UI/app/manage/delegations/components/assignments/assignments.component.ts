import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { DelegationAssignmentListItem } from 'services/delegationassignments';

@Component({
  selector: 'ww-delegation-assignment-list',
  templateUrl: 'assignments.template.html',
  styleUrls: ['assignments.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class DelegationAssignmentComponent extends BaseListComponent<DelegationAssignmentListItem> {
}
