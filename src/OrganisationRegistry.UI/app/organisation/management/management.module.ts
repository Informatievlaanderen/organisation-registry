import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationManagementRoutingModule } from './management-routing.module';

import { OrganisationManagementComponent } from './management.component';
import { OrganisationManagementOverviewComponent } from './overview';
import { OrganisationSyncServiceModule } from "services/organisationsync";


@NgModule({
  imports: [
    SharedModule,
    OrganisationManagementRoutingModule,
    OrganisationSyncServiceModule,
  ],
  declarations: [
    OrganisationManagementComponent,
    OrganisationManagementOverviewComponent,
  ],
  exports: [
    OrganisationManagementRoutingModule
  ]
})
export class OrganisationManagementModule { }
