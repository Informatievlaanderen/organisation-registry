import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ProjectionStatesServiceModule } from 'services/projectionstates';
import { ProjectionStatesRoutingModule } from './projection-states-routing.module';

import { ProjectionStateOverviewComponent } from './overview';
import { ProjectionStateDetailComponent } from './detail';

import {
  ProjectionStateListComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    ProjectionStatesRoutingModule,
    ProjectionStatesServiceModule
  ],
  declarations: [
    ProjectionStateOverviewComponent,
    ProjectionStateDetailComponent,
    ProjectionStateListComponent
  ],
  exports: [
    ProjectionStatesRoutingModule
  ]
})
export class SystemProjectionStatesModule { }
