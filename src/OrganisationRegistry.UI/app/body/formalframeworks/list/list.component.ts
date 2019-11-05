import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { BodyFormalFrameworkListItem } from 'services/bodyformalframeworks';

@Component({
  selector: 'ww-body-formal-framework-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class BodyFormalFrameworksListComponent extends BaseListComponent<BodyFormalFrameworkListItem> {
  @Input('canEdit') canEdit: boolean;
}
