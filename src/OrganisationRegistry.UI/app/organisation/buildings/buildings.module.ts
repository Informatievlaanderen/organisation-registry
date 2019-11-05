import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BuildingsServiceModule } from 'services/buildings';
import { OrganisationBuildingsServiceModule } from 'services/organisationbuildings';

import { OrganisationBuildingsRoutingModule } from './buildings-routing.module';

import { OrganisationBuildingsComponent } from './buildings.component';
import { OrganisationBuildingsOverviewComponent } from './overview';
import { OrganisationBuildingsListComponent } from './list';
import { OrganisationBuildingsFilterComponent } from './filter';
import { OrganisationBuildingsCreateOrganisationBuildingComponent } from './create';
import { OrganisationBuildingsUpdateOrganisationBuildingComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationBuildingsRoutingModule,
    OrganisationBuildingsServiceModule,
    BuildingsServiceModule
  ],
  declarations: [
    OrganisationBuildingsComponent,
    OrganisationBuildingsOverviewComponent,
    OrganisationBuildingsListComponent,
    OrganisationBuildingsFilterComponent,
    OrganisationBuildingsCreateOrganisationBuildingComponent,
    OrganisationBuildingsUpdateOrganisationBuildingComponent
  ],
  exports: [
    OrganisationBuildingsRoutingModule
  ]
})
export class OrganisationBuildingsModule { }
