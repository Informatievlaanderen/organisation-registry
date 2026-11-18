import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OidcService } from 'core/auth';

import { PersonFunctionListItem } from 'services/peoplefunctions';

@Component({
  selector: 'ww-person-function-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class PeopleFunctionsListComponent extends BaseListComponent<PersonFunctionListItem> {
  isLoggedIn: Observable<boolean>;

  constructor(oidcService: OidcService) {
    super();
    this.isLoggedIn = oidcService.isLoggedIn;
  }
}
