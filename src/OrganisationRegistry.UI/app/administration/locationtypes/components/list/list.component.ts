import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { LocationType } from 'services/locationtypes';

@Component({
  selector: 'ww-location-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class LocationTypeListComponent extends BaseListComponent<LocationType> {
}
