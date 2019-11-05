import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { CapacitiesServiceModule } from 'services/capacities';
import { CapacityPersonsRoutingModule } from './capacities-routing.module';

import { CapacityDetailComponent } from './detail';
import { CapacityOverviewComponent } from './overview';

import {
  CapacityListComponent,
  CapacityFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    CapacityPersonsRoutingModule,
    CapacitiesServiceModule
  ],
  declarations: [
    CapacityDetailComponent,
    CapacityListComponent,
    CapacityOverviewComponent,
    CapacityFilterComponent
  ],
  exports: [
    CapacityPersonsRoutingModule
  ]
})
export class AdministrationCapacitiesModule { }
