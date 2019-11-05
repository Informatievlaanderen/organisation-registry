import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { BodyMandateListItem } from 'services/bodymandates';

@Component({
  selector: 'ww-body-mandate-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class BodyMandatesListComponent extends BaseListComponent<BodyMandateListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
