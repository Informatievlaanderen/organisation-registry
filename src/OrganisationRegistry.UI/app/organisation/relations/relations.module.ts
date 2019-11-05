import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationRelationTypesServiceModule } from 'services/organisationrelationtypes';
import { OrganisationRelationsServiceModule } from 'services/organisationrelations';

import { OrganisationRelationsRoutingModule } from './relations-routing.module';

import { OrganisationRelationsComponent } from './relations.component';
import { OrganisationRelationsOverviewComponent } from './overview';
import { OrganisationRelationsListComponent } from './list';
import { OrganisationRelationsFilterComponent } from './filter';
import { OrganisationRelationsCreateOrganisationRelationComponent } from './create';
import { OrganisationRelationsUpdateOrganisationRelationComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationRelationsRoutingModule,
    OrganisationRelationsServiceModule,
    OrganisationRelationTypesServiceModule
  ],
  declarations: [
    OrganisationRelationsComponent,
    OrganisationRelationsOverviewComponent,
    OrganisationRelationsListComponent,
    OrganisationRelationsFilterComponent,
    OrganisationRelationsCreateOrganisationRelationComponent,
    OrganisationRelationsUpdateOrganisationRelationComponent
  ],
  exports: [
    OrganisationRelationsRoutingModule
  ]
})
export class OrganisationRelationsModule { }
