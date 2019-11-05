import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { Capacity } from 'services/capacities';

@Component({
  selector: 'ww-capacity-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class CapacityListComponent extends BaseListComponent<Capacity> {
}
