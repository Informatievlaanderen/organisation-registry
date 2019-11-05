import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { OrganisationClassification } from 'services/organisationclassifications';

@Component({
  selector: 'ww-organisation-classification-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationClassificationListComponent extends BaseListComponent<OrganisationClassification> {
}
