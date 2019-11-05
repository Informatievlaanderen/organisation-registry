import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { PersonMandateListItem } from 'services/peoplemandates';

@Component({
  selector: 'ww-person-mandate-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class PeopleMandatesListComponent extends BaseListComponent<PersonMandateListItem> {
}
