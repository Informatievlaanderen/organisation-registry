import { Component } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';
import { BodySearchListItem } from 'services/search/body';

@Component({
  selector: 'ww-body-search-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy']
})
export class BodySearchListComponent extends BaseListComponent<BodySearchListItem> { }
