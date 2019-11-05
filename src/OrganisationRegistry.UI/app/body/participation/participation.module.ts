import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodiesServiceModule } from 'services/bodies';

import { BodyParticipationRoutingModule } from './participation-routing.module';

import { ReportsServiceModule } from 'services/reports';

import { BodyParticipationComponent } from './participation.component';
import { BodyParticipationOverviewComponent } from './overview';
import { BodyParticipationListComponent } from './list';
import { BodyParticipationFilterComponent } from './filter';
import { BodyParticipationManageComponent } from './manage';

@NgModule({
  imports: [
    SharedModule,
    BodyParticipationRoutingModule,
    ReportsServiceModule,
    BodiesServiceModule
  ],
  declarations: [
    BodyParticipationComponent,
    BodyParticipationOverviewComponent,
    BodyParticipationListComponent,
    BodyParticipationFilterComponent,
    BodyParticipationManageComponent
  ],
  exports: [
    BodyParticipationRoutingModule
  ]
})
export class BodyParticipationModule { }
