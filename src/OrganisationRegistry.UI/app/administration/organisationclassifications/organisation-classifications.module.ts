import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationClassificationsServiceModule } from 'services/organisationclassifications';
import { OrganisationClassificationsRoutingModule } from './organisation-classifications-routing.module';

import { OrganisationClassificationDetailComponent } from './detail';
import { OrganisationClassificationOverviewComponent } from './overview';

import {
  OrganisationClassificationListComponent,
  OrganisationClassificationFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    OrganisationClassificationsRoutingModule,
    OrganisationClassificationsServiceModule
  ],
  declarations: [
    OrganisationClassificationDetailComponent,
    OrganisationClassificationListComponent,
    OrganisationClassificationOverviewComponent,
    OrganisationClassificationFilterComponent
  ],
  exports: [
    OrganisationClassificationsRoutingModule
  ]
})
export class AdministrationOrganisationClassificationsModule { }
