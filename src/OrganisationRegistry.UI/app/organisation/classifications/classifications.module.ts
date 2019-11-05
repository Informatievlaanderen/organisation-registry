import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationClassificationsServiceModule } from 'services/organisationclassifications';
import { OrganisationClassificationTypesServiceModule } from 'services/organisationclassificationtypes';
import { OrganisationOrganisationClassificationsServiceModule } from 'services/organisationorganisationclassifications';

import { OrganisationOrganisationClassificationsRoutingModule } from './classifications-routing.module';

import { OrganisationOrganisationClassificationsComponent } from './classifications.component';
import { OrganisationOrganisationClassificationsOverviewComponent } from './overview';
import { OrganisationOrganisationClassificationsListComponent } from './list';
import { OrganisationOrganisationClassificationsFilterComponent } from './filter';
import { OrganisationOrganisationClassificationsCreateOrganisationOrganisationClassificationComponent } from './create';
import { OrganisationOrganisationClassificationsUpdateOrganisationOrganisationClassificationComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationOrganisationClassificationsRoutingModule,
    OrganisationOrganisationClassificationsServiceModule,
    OrganisationClassificationsServiceModule,
    OrganisationClassificationTypesServiceModule
  ],
  declarations: [
    OrganisationOrganisationClassificationsComponent,
    OrganisationOrganisationClassificationsOverviewComponent,
    OrganisationOrganisationClassificationsListComponent,
    OrganisationOrganisationClassificationsFilterComponent,
    OrganisationOrganisationClassificationsCreateOrganisationOrganisationClassificationComponent,
    OrganisationOrganisationClassificationsUpdateOrganisationOrganisationClassificationComponent
  ],
  exports: [
    OrganisationOrganisationClassificationsRoutingModule
  ]
})
export class OrganisationClassificationsModule { }
