import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationRelationTypesServiceModule } from 'services/organisationrelationtypes';
import { OrganisationRelationTypesRoutingModule } from './organisation-relation-types-routing.module';

import { OrganisationRelationTypeDetailComponent } from './detail';
import { OrganisationRelationTypeOverviewComponent } from './overview';

import {
  OrganisationRelationTypeListComponent,
  OrganisationRelationTypeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    OrganisationRelationTypesRoutingModule,
    OrganisationRelationTypesServiceModule
  ],
  declarations: [
    OrganisationRelationTypeDetailComponent,
    OrganisationRelationTypeListComponent,
    OrganisationRelationTypeOverviewComponent,
    OrganisationRelationTypeFilterComponent
  ],
  exports: [
    OrganisationRelationTypesRoutingModule
  ]
})
export class AdministrationOrganisationRelationTypesModule { }
