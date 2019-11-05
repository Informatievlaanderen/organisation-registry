import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { Person } from 'services/people';

@Component({
  selector: 'ww-people-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage', 'exportCsv']
})
export class PersonListComponent extends BaseListComponent<Person> {

}
