import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { Organisation } from 'services/organisations';

@Component({
  selector: 'ww-organisation-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage', 'exportCsv']
})
export class OrganisationListComponent extends BaseListComponent<Organisation> {

}
