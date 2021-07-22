import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { KboServiceModule } from 'services/kbo';
import { OrganisationsServiceModule } from 'services/organisations';
import { TogglesServiceModule, FeaturesServiceModule } from 'services';

import { OrganisationRoutingModule } from './organisation-routing.module';

import { OrganisationOverviewComponent, OrganisationListComponent, OrganisationFilterComponent } from './overview';
import { CreateOrganisationComponent, CreateOrganisationFormComponent } from './create';
import { OrganisationDetailModule } from './detail';

@NgModule({
  imports: [
    SharedModule,
    KboServiceModule,
    TogglesServiceModule,
    FeaturesServiceModule,
    OrganisationRoutingModule,
    OrganisationsServiceModule,
    OrganisationDetailModule
  ],
  declarations: [
    OrganisationOverviewComponent,
    OrganisationListComponent,
    OrganisationFilterComponent,
    CreateOrganisationComponent,
    CreateOrganisationFormComponent
  ],
  exports: [
    OrganisationRoutingModule
  ]
})
export class OrganisationModule { }
