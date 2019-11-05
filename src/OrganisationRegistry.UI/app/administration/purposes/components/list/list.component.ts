import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { Purpose } from 'services/purposes';

@Component({
  selector: 'ww-purpose-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class PurposeListComponent extends BaseListComponent<Purpose> {
}
