import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { RegulationTypesServiceModule } from 'services/regulationtypes';
import { RegulationTypesRoutingModule } from './regulation-types-routing.module';

import { RegulationTypeDetailComponent } from './detail';
import { RegulationTypeOverviewComponent } from './overview';

import {
  RegulationTypeListComponent,
  RegulationTypeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    RegulationTypesRoutingModule,
    RegulationTypesServiceModule
  ],
  declarations: [
    RegulationTypeDetailComponent,
    RegulationTypeListComponent,
    RegulationTypeOverviewComponent,
    RegulationTypeFilterComponent
  ],
  exports: [
    RegulationTypesRoutingModule
  ]
})
export class AdministrationRegulationTypesModule { }
