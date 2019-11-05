import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { PersonCapacityListItem } from 'services/peoplecapacities';

@Component({
  selector: 'ww-person-capacity-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class PeopleCapacitiesListComponent extends BaseListComponent<PersonCapacityListItem> {
}
