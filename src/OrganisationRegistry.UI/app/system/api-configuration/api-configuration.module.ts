import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ApiConfigurationServiceModule } from 'services/api-configuration';
import { ApiConfigurationRoutingModule } from './api-configuration-routing.module';

import { ApiConfigurationOverviewComponent } from './overview';

@NgModule({
  imports: [
    SharedModule,
    ApiConfigurationRoutingModule,
    ApiConfigurationServiceModule
  ],
  declarations: [
    ApiConfigurationOverviewComponent
  ],
  exports: [
    ApiConfigurationRoutingModule
  ]
})
export class SystemApiConfigurationModule { }
