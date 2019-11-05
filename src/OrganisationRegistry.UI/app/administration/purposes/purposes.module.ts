import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { PurposesServiceModule } from 'services/purposes';
import { PurposesRoutingModule } from './purposes-routing.module';

import { PurposeDetailComponent } from './detail';
import { PurposeOverviewComponent } from './overview';

import {
  PurposeListComponent,
  PurposeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    PurposesRoutingModule,
    PurposesServiceModule
  ],
  declarations: [
    PurposeDetailComponent,
    PurposeListComponent,
    PurposeOverviewComponent,
    PurposeFilterComponent
  ],
  exports: [
    PurposesRoutingModule
  ]
})
export class AdministrationPurposesModule { }
