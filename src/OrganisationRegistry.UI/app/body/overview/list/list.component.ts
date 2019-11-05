import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { Body } from 'services/bodies';

@Component({
  selector: 'ww-body-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage', 'exportCsv']
})
export class BodyListComponent extends BaseListComponent<Body> {

}
