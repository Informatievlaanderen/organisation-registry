import { Component } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationFormalFrameworkListItem } from 'services/organisationformalframeworks';

@Component({
  selector: 'ww-organisation-formal-framework-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationFormalFrameworksListComponent extends BaseListComponent<OrganisationFormalFrameworkListItem> {}
