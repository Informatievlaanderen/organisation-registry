import { Component} from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { RegulationTheme } from 'services/regulation-themes';

@Component({
  selector: 'ww-regulation-theme-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class RegulationThemeListComponent extends BaseListComponent<RegulationTheme> {
}
