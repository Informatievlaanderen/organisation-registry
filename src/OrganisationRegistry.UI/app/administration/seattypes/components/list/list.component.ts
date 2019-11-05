import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { SeatType } from 'services/seattypes';

@Component({
  selector: 'ww-seat-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class SeatTypeListComponent extends BaseListComponent<SeatType> {
}
