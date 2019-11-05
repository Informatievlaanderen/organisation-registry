import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { OrganisationChild } from 'services/organisations';

@Component({
  selector: 'ww-organisation-child-list',
  templateUrl: 'children.template.html',
  styleUrls: ['children.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationInfoChildrenComponent extends BaseListComponent<OrganisationChild> {
}
