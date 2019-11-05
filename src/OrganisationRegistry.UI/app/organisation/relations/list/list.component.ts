import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationRelationListItem } from 'services/organisationrelations';

@Component({
  selector: 'ww-organisation-relation-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationRelationsListComponent extends BaseListComponent<OrganisationRelationListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
