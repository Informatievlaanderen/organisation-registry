import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { SystemEventsModule } from './../events';
import { SystemProjectionsModule } from './../projections';
import { SystemProjectionStatesModule } from './../projectionstates';
import { SystemTogglesModule } from './../toggles';
import { SystemApiConfigurationModule } from './../api-configuration';
import { SystemExceptionsModule } from './../exceptions';
import { SystemStatsModule } from './../stats';
import { SystemConfigurationValuesModule } from './../configurationvalues';

import { TerminatedInKboModule } from './../terminated-in-kbo/terminated-in-kbo.module';

import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardOverviewComponent } from './overview';

@NgModule({
  imports: [
    SharedModule,
    DashboardRoutingModule,

    SystemEventsModule,
    SystemProjectionsModule,
    SystemProjectionStatesModule,
    SystemTogglesModule,
    SystemApiConfigurationModule,
    SystemExceptionsModule,
    SystemStatsModule,
    SystemConfigurationValuesModule,
    TerminatedInKboModule
  ],
  declarations: [
    DashboardOverviewComponent
  ],
  exports: [
    DashboardRoutingModule
  ]
})
export class SystemDashboardModule { }
