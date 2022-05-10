import {Component, Output, EventEmitter} from '@angular/core';

import {BaseListComponent} from 'shared/components/list';

import {KeyType} from 'services/keytypes';

@Component({
  selector: 'ww-key-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class KeyTypeListComponent extends BaseListComponent<KeyType> {

  @Output()
  removeKeyTypeClicked: EventEmitter<KeyType> = new EventEmitter<KeyType>();

  remove(id: KeyType) {
    this.removeKeyTypeClicked.emit(id);
  }
}
