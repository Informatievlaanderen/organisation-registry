import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationParentListItem } from 'services/organisationparents';

@Component({
  selector: 'ww-organisation-parent-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationParentsListComponent extends BaseListComponent<OrganisationParentListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
