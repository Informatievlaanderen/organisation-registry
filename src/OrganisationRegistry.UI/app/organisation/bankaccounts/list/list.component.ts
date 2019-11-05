import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Observable } from 'rxjs/Observable';

import { BaseListComponent } from 'shared/components/list';
import { OrganisationBankAccountListItem } from 'services/organisationbankaccounts';

@Component({
  selector: 'ww-organisation-bank-account-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class OrganisationBankAccountsListComponent extends BaseListComponent<OrganisationBankAccountListItem> {
  @Input('canEdit') canEdit: Observable<boolean>;
}
