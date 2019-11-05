import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { KeyType } from 'services/keytypes';

@Component({
  selector: 'ww-key-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class KeyTypeListComponent extends BaseListComponent<KeyType> {
}
