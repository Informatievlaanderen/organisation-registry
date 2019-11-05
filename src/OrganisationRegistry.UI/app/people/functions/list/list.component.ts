import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { PersonFunctionListItem } from 'services/peoplefunctions';

@Component({
  selector: 'ww-person-function-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class PeopleFunctionsListComponent extends BaseListComponent<PersonFunctionListItem> {
}
