import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { PeopleMandatesServiceModule } from 'services/peoplemandates';

import { PeopleMandatesRoutingModule } from './mandates-routing.module';

import { PeopleMandatesComponent } from './mandates.component';
import { PeopleMandatesOverviewComponent } from './overview';
import { PeopleMandatesListComponent } from './list';

@NgModule({
  imports: [
    SharedModule,
    PeopleMandatesRoutingModule,
    PeopleMandatesServiceModule
  ],
  declarations: [
    PeopleMandatesComponent,
    PeopleMandatesOverviewComponent,
    PeopleMandatesListComponent,
  ],
  exports: [
    PeopleMandatesRoutingModule
  ]
})
export class PeopleMandatesModule { }
