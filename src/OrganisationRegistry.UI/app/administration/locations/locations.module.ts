import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { LocationsServiceModule } from 'services/locations';
import { LocationsRoutingModule } from './locations-routing.module';

import { LocationDetailComponent } from './detail';
import { LocationOverviewComponent } from './overview';

import {
  LocationListComponent,
  LocationFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    LocationsRoutingModule,
    LocationsServiceModule
  ],
  declarations: [
    LocationDetailComponent,
    LocationListComponent,
    LocationOverviewComponent,
    LocationFilterComponent
  ],
  exports: [
    LocationsRoutingModule
  ]
})
export class AdministrationLocationsModule { }
