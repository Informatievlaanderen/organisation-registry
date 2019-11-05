import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationBodyListItem } from 'services/organisationbodies';

@Component({
  selector: 'ww-organisation-body-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationBodiesListComponent extends BaseListComponent<OrganisationBodyListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
