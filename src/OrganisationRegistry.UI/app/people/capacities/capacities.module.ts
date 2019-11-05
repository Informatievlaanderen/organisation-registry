import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { PeopleCapacitiesServiceModule } from 'services/peoplecapacities';

import { PeopleCapacitiesRoutingModule } from './capacities-routing.module';

import { PeopleCapacitiesComponent } from './capacities.component';
import { PeopleCapacitiesOverviewComponent } from './overview';
import { PeopleCapacitiesListComponent } from './list';

@NgModule({
  imports: [
    SharedModule,
    PeopleCapacitiesRoutingModule,
    PeopleCapacitiesServiceModule
  ],
  declarations: [
    PeopleCapacitiesComponent,
    PeopleCapacitiesOverviewComponent,
    PeopleCapacitiesListComponent,
  ],
  exports: [
    PeopleCapacitiesRoutingModule
  ]
})
export class PeopleCapacitiesModule { }
