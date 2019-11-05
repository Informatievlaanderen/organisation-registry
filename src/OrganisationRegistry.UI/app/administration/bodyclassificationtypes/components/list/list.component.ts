import { Component } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { BodyClassificationType } from 'services/bodyclassificationtypes';

@Component({
  selector: 'ww-body-classification-type-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class BodyClassificationTypeListComponent extends BaseListComponent<BodyClassificationType> {
}
