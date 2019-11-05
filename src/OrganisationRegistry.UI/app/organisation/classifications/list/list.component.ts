import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationOrganisationClassificationListItem } from 'services/organisationorganisationclassifications';

@Component({
  selector: 'ww-organisation-organisationclassification-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationOrganisationClassificationsListComponent extends BaseListComponent<OrganisationOrganisationClassificationListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
