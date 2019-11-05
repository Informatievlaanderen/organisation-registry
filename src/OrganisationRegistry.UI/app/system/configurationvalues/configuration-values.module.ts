import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ConfigurationValuesServiceModule } from 'services/configurationvalues';
import { ConfigurationValuesRoutingModule } from './configuration-values-routing.module';

import { ConfigurationValueDetailComponent } from './detail';
import { ConfigurationValueOverviewComponent } from './overview';

import {
  ConfigurationValueListComponent,
  ConfigurationValueFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    ConfigurationValuesRoutingModule,
    ConfigurationValuesServiceModule
  ],
  declarations: [
    ConfigurationValueDetailComponent,
    ConfigurationValueListComponent,
    ConfigurationValueOverviewComponent,
    ConfigurationValueFilterComponent
  ],
  exports: [
    ConfigurationValuesRoutingModule
  ]
})
export class SystemConfigurationValuesModule { }
