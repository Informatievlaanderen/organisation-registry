import { Component } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';
import { PersonSearchListItem } from 'services/search/person';

@Component({
  selector: 'ww-person-search-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy']
})
export class PersonSearchListComponent extends BaseListComponent<PersonSearchListItem> { }
