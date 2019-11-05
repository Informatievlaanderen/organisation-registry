import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { SeatTypesServiceModule } from 'services/seattypes';
import { SeatTypesRoutingModule } from './seat-types-routing.module';

import { SeatTypeDetailComponent } from './detail';
import { SeatTypeOverviewComponent } from './overview';

import {
  SeatTypeListComponent,
  SeatTypeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    SeatTypesRoutingModule,
    SeatTypesServiceModule
  ],
  declarations: [
    SeatTypeDetailComponent,
    SeatTypeListComponent,
    SeatTypeOverviewComponent,
    SeatTypeFilterComponent
  ],
  exports: [
    SeatTypesRoutingModule
  ]
})
export class AdministrationSeatTypesModule { }
