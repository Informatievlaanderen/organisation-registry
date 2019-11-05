import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { LocationsServiceModule } from 'services/locations';
import { OrganisationLocationsServiceModule } from 'services/organisationlocations';

import { OrganisationLocationsRoutingModule } from './locations-routing.module';

import { OrganisationLocationsComponent } from './locations.component';
import { OrganisationLocationsOverviewComponent } from './overview';
import { OrganisationLocationsListComponent } from './list';
import { OrganisationLocationsFilterComponent } from './filter';
import { OrganisationLocationsCreateOrganisationLocationComponent } from './create';
import { OrganisationLocationsUpdateOrganisationLocationComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationLocationsRoutingModule,
    OrganisationLocationsServiceModule,
    LocationsServiceModule
  ],
  declarations: [
    OrganisationLocationsComponent,
    OrganisationLocationsOverviewComponent,
    OrganisationLocationsListComponent,
    OrganisationLocationsFilterComponent,
    OrganisationLocationsCreateOrganisationLocationComponent,
    OrganisationLocationsUpdateOrganisationLocationComponent
  ],
  exports: [
    OrganisationLocationsRoutingModule
  ]
})
export class OrganisationLocationsModule { }
