import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ReportsServiceModule } from 'services/reports';
import { BuildingOrganisationsRoutingModule } from './building-organisations-routing.module';

import { BuildingOverviewComponent } from './overview';
import { BuildingDetailComponent } from './detail';

import {
  BuildingListComponent,
  BuildingFilterComponent,
  BuildingOrganisationListComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    BuildingOrganisationsRoutingModule,
    ReportsServiceModule
  ],
  declarations: [
    BuildingOverviewComponent,
    BuildingListComponent,
    BuildingFilterComponent,
    BuildingDetailComponent,
    BuildingOrganisationListComponent
  ],
  exports: [
    BuildingOrganisationsRoutingModule
  ]
})
export class ReportBuildingOrganisationsModule { }
