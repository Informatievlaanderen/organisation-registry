import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { ContactType } from 'services/contacttypes';

@Component({
  selector: 'ww-contact-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class ContactTypeListComponent extends BaseListComponent<ContactType> {
}
