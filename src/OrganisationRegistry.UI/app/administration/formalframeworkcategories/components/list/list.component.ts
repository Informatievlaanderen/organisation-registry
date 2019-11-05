import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { FormalFrameworkCategory } from 'services/formalframeworkcategories';

@Component({
  selector: 'ww-formal-framework-category-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class FormalFrameworkCategoryListComponent extends BaseListComponent<FormalFrameworkCategory> {
}
