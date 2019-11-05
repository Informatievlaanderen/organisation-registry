import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationClassificationTypesServiceModule } from 'services/organisationclassificationtypes';
import { OrganisationClassificationTypesRoutingModule } from './organisation-classification-types-routing.module';

import { OrganisationClassificationTypeDetailComponent } from './detail';
import { OrganisationClassificationTypeOverviewComponent } from './overview';

import {
  OrganisationClassificationTypeListComponent,
  OrganisationClassificationTypeFilterComponent
} from './components';

@NgModule({
  imports: [
    SharedModule,
    OrganisationClassificationTypesRoutingModule,
    OrganisationClassificationTypesServiceModule
  ],
  declarations: [
    OrganisationClassificationTypeDetailComponent,
    OrganisationClassificationTypeListComponent,
    OrganisationClassificationTypeOverviewComponent,
    OrganisationClassificationTypeFilterComponent
  ],
  exports: [
    OrganisationClassificationTypesRoutingModule
  ]
})
export class AdministrationOrganisationClassificationTypesModule { }
