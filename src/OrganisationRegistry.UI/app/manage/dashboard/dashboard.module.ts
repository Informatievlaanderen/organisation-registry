import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ManageDelegationsModule } from './../delegations';

import { DashboardRoutingModule } from './dashboard-routing.module';

import { DashboardOverviewComponent } from './overview';

@NgModule({
  imports: [
    SharedModule,
    DashboardRoutingModule,

    ManageDelegationsModule
  ],
  declarations: [
    DashboardOverviewComponent
  ],
  exports: [
    DashboardRoutingModule
  ]
})
export class ManageDashboardModule { }
