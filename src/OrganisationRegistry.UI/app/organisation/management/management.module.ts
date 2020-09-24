import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationSyncRoutingModule } from './sync-routing.module';

import { OrganisationSyncComponent } from './sync.component';
import { OrganisationSyncOverviewComponent } from './overview';
import { OrganisationSyncServiceModule } from "services/organisationsync";


@NgModule({
  imports: [
    SharedModule,
    OrganisationSyncRoutingModule,
    OrganisationSyncServiceModule,
  ],
  declarations: [
    OrganisationSyncComponent,
    OrganisationSyncOverviewComponent,
  ],
  exports: [
    OrganisationSyncRoutingModule
  ]
})
export class OrganisationSyncModule { }
