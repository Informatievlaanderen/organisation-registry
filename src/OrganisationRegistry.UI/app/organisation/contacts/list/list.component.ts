import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationContactListItem } from 'services/organisationcontacts';

@Component({
  selector: 'ww-organisation-contact-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationContactsListComponent extends BaseListComponent<OrganisationContactListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
