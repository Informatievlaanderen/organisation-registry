import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { LabelType } from 'services/labeltypes';

@Component({
  selector: 'ww-label-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class LabelTypeListComponent extends BaseListComponent<LabelType> {
}
