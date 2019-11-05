import { Component, Input, Output, EventEmitter } from '@angular/core';

import { BaseListComponent } from 'shared/components/list';

import { ConfigurationValue } from 'services/configurationvalues';

@Component({
  selector: 'ww-configuration-value-list',
  templateUrl: 'list.template.html',
  styleUrls: ['list.style.css'],
  inputs: ['items', 'isBusy'],
  outputs: ['changePage']
})
export class ConfigurationValueListComponent extends BaseListComponent<ConfigurationValue> {
}
