import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { MandateRoleType } from 'services/mandateroletypes';

@Component({
  selector: 'ww-mandate-role-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class MandateRoleTypeListComponent extends BaseListComponent<MandateRoleType> {
}
