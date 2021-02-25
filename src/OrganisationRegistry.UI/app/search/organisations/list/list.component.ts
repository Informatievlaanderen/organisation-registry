import { Component } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationDocument } from 'services/search/organisation';

@Component({
  selector: 'ww-organisation-search-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationSearchListComponent extends BaseListComponent<OrganisationDocument> { }
