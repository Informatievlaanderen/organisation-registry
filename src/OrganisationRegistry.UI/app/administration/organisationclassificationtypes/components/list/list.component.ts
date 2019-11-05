import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { OrganisationClassificationType } from 'services/organisationclassificationtypes';

@Component({
  selector: 'ww-organisation-classification-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationClassificationTypeListComponent extends BaseListComponent<OrganisationClassificationType> {
}
