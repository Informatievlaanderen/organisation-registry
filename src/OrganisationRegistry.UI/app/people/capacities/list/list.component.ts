import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OidcService } from 'core/auth';

import { PersonCapacityListItem } from 'services/peoplecapacities';

@Component({
  selector: 'ww-person-capacity-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class PeopleCapacitiesListComponent extends BaseListComponent<PersonCapacityListItem> {
  isLoggedIn: Observable<boolean>;

  constructor(oidcService: OidcService) {
    super();
    this.isLoggedIn = oidcService.isLoggedIn;
  }
}
