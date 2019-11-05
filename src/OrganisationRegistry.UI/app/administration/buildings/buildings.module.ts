import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BuildingsServiceModule } from 'services/buildings';
import { BuildingsRoutingModule } from './buildings-routing.module';

import { BuildingDetailComponent } from './detail';
import { BuildingOverviewComponent } from './overview';

import {
  BuildingListComponent,
  BuildingFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    BuildingsRoutingModule,
    BuildingsServiceModule
  ],
  declarations: [
    BuildingDetailComponent,
    BuildingListComponent,
    BuildingOverviewComponent,
    BuildingFilterComponent
  ],
  exports: [
    BuildingsRoutingModule
  ]
})
export class AdministrationBuildingsModule { }
