import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { CapacitiesServiceModule } from 'services/capacities';
import { PeopleFunctionsServiceModule } from 'services/peoplefunctions';
import { OrganisationCapacitiesServiceModule } from 'services/organisationcapacities';

import { OrganisationCapacitiesRoutingModule } from './capacities-routing.module';

import { OrganisationCapacitiesComponent } from './capacities.component';
import { OrganisationCapacitiesOverviewComponent } from './overview';
import { OrganisationCapacitiesListComponent } from './list';
import { OrganisationCapacitiesFilterComponent } from './filter';
import { OrganisationCapacitiesCreateOrganisationCapacityComponent } from './create';
import { OrganisationCapacitiesUpdateOrganisationCapacityComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationCapacitiesRoutingModule,
    OrganisationCapacitiesServiceModule,
    CapacitiesServiceModule,
    PeopleFunctionsServiceModule
  ],
  declarations: [
    OrganisationCapacitiesComponent,
    OrganisationCapacitiesOverviewComponent,
    OrganisationCapacitiesListComponent,
    OrganisationCapacitiesFilterComponent,
    OrganisationCapacitiesCreateOrganisationCapacityComponent,
    OrganisationCapacitiesUpdateOrganisationCapacityComponent
  ],
  exports: [
    OrganisationCapacitiesRoutingModule
  ]
})
export class OrganisationCapacitiesModule { }
