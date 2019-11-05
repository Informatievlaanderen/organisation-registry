import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { TasksServiceModule } from 'services/tasks';
import { ProjectionsServiceModule } from 'services/projections';
import { ProjectionsRoutingModule } from './projections-routing.module';

// import { DetailComponent } from './detail';
import { ProjectionOverviewComponent } from './overview';

// import {
//   ListComponent,
//   FilterComponent
// } from './components/';

@NgModule({
  imports: [
    SharedModule,
    ProjectionsRoutingModule,
    ProjectionsServiceModule,
    TasksServiceModule
  ],
  declarations: [
    ProjectionOverviewComponent
  ],
  exports: [
    ProjectionsRoutingModule
  ]
})
export class SystemProjectionsModule { }
