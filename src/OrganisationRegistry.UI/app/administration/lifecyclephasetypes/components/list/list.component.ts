import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { LifecyclePhaseType } from 'services/lifecyclephasetypes';

@Component({
  selector: 'ww-lifecycle-phase-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class LifecyclePhaseTypeListComponent extends BaseListComponent<LifecyclePhaseType> {
}
