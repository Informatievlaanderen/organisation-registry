import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { OrganisationRelationType } from 'services/organisationrelationtypes';

@Component({
  selector: 'ww-organisation-relation-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationRelationTypeListComponent extends BaseListComponent<OrganisationRelationType> {
}
