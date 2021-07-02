import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { RegulationType } from 'services/regulationtypes';

@Component({
  selector: 'ww-regulation-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class RegulationTypeListComponent extends BaseListComponent<RegulationType> {
}
