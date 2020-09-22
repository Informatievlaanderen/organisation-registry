import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationSyncServiceModule } from 'services/organisationsync';
import { TerminatedInKboRoutingModule} from './terminated-in-kbo-routing.module';

import { TerminatedInKboOverviewComponent } from './overview';

import {
  TerminatedInKboListComponent,
  TerminatedInKboFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    TerminatedInKboRoutingModule,
    OrganisationSyncServiceModule
  ],
  declarations: [
    TerminatedInKboListComponent,
    TerminatedInKboFilterComponent,
    TerminatedInKboOverviewComponent
  ],
  exports: [
    TerminatedInKboRoutingModule
  ]
})
export class TerminatedInKboModule { }
