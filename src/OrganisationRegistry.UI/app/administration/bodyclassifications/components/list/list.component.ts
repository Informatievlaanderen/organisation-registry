import { Component } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { BodyClassification } from 'services/bodyclassifications';

@Component({
  selector: 'ww-body-classification-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class BodyClassificationListComponent extends BaseListComponent<BodyClassification> {
}
