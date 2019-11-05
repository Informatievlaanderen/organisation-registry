import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { LocationTypesServiceModule } from 'services/locationtypes';
import { LocationTypesRoutingModule } from './location-types-routing.module';

import { LocationTypeDetailComponent } from './detail';
import { LocationTypeOverviewComponent } from './overview';

import {
  LocationTypeListComponent,
  LocationTypeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    LocationTypesRoutingModule,
    LocationTypesServiceModule
  ],
  declarations: [
    LocationTypeDetailComponent,
    LocationTypeListComponent,
    LocationTypeOverviewComponent,
    LocationTypeFilterComponent
  ],
  exports: [
    LocationTypesRoutingModule
  ]
})
export class AdministrationLocationTypesModule { }
