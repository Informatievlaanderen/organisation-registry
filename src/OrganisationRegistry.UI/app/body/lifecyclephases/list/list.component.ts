import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { BodyLifecyclePhaseListItem } from 'services/bodylifecyclephases';

@Component({
  selector: 'ww-body-lifecycle-phase-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class BodyLifecyclePhasesListComponent extends BaseListComponent<BodyLifecyclePhaseListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
