import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { OrganisationTermination } from 'services/organisationsync';

@Component({
  selector: 'ww-terminated-in-kbo-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class TerminatedInKboListComponent extends BaseListComponent<OrganisationTermination> {
}
