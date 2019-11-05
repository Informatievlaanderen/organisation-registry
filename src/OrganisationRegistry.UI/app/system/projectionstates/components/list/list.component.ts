import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { ProjectionState } from 'services/projectionstates';

@Component({
  selector: 'ww-projection-state-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class ProjectionStateListComponent extends BaseListComponent<ProjectionState> {
}
