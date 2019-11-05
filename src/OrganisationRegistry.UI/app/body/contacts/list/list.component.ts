import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { BodyContactListItem } from 'services/bodycontacts';

@Component({
  selector: 'ww-body-contact-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class BodyContactsListComponent extends BaseListComponent<BodyContactListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
