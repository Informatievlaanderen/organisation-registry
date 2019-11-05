import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { DelegationListItem } from 'services/delegations';

@Component({
  selector: 'ww-delegation-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class DelegationListItemListComponent extends BaseListComponent<DelegationListItem> {
  @Input('hasNonDelegatedDelegations') hasNonDelegatedDelegations: boolean;
}
