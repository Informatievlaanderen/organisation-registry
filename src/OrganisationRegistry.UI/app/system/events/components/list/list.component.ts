import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { EventData } from 'services/events';

@Component({
  selector: 'ww-event-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class EventDataListComponent extends BaseListComponent<EventData> {
}
