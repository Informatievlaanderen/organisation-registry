import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { StatsServiceModule } from 'services/stats';
import { StatsRoutingModule } from './stats-routing.module';

import { StatsOverviewComponent } from './overview';

@NgModule({
  imports: [
    SharedModule,
    StatsRoutingModule,
    StatsServiceModule
  ],
  declarations: [
    StatsOverviewComponent
  ],
  exports: [
    StatsRoutingModule
  ]
})
export class SystemStatsModule { }
