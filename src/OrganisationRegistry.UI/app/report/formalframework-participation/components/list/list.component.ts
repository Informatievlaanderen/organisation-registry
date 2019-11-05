import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { FormalFramework } from 'services/formalframeworks';

@Component({
  selector: 'ww-report-formal-framework-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage', 'exportCsv']
})
export class FormalFrameworkListComponent extends BaseListComponent<FormalFramework> {

}
