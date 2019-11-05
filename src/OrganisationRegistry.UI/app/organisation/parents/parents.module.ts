import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationsServiceModule } from 'services/organisations';
import { OrganisationParentsServiceModule } from 'services/organisationparents';

import { OrganisationParentsRoutingModule } from './parents-routing.module';

import { OrganisationParentsComponent } from './parents.component';
import { OrganisationParentsOverviewComponent } from './overview';
import { OrganisationParentsListComponent } from './list';
import { OrganisationParentsCreateOrganisationParentComponent } from './create';
import { OrganisationParentsUpdateOrganisationParentComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationParentsRoutingModule,
    OrganisationParentsServiceModule,
    OrganisationsServiceModule
  ],
  declarations: [
    OrganisationParentsComponent,
    OrganisationParentsOverviewComponent,
    OrganisationParentsListComponent,
    OrganisationParentsCreateOrganisationParentComponent,
    OrganisationParentsUpdateOrganisationParentComponent
  ],
  exports: [
    OrganisationParentsRoutingModule
  ]
})
export class OrganisationParentsModule { }
