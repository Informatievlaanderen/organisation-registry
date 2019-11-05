import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { BodySeatListItem } from 'services/bodyseats';

@Component({
  selector: 'ww-body-seat-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class BodySeatsListComponent extends BaseListComponent<BodySeatListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
