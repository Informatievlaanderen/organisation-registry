import { Component } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { RegulationSubTheme } from 'services/regulation-sub-themes';

@Component({
  selector: 'ww-regulation-sub-theme-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class RegulationSubThemeListComponent extends BaseListComponent<RegulationSubTheme> {
}
