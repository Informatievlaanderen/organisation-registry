import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { BodyBodyClassificationListItem } from 'services/bodybodyclassifications';

@Component({
  selector: 'ww-body-bodyclassification-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class BodyBodyClassificationsListComponent extends BaseListComponent<BodyBodyClassificationListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
