import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { MandateRoleTypesServiceModule } from 'services/mandateroletypes';
import { MandateRoleTypesRoutingModule } from './mandate-role-types-routing.module';

import { MandateRoleTypeDetailComponent } from './detail';
import { MandateRoleTypeOverviewComponent } from './overview';

import {
  MandateRoleTypeListComponent,
  MandateRoleTypeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    MandateRoleTypesRoutingModule,
    MandateRoleTypesServiceModule
  ],
  declarations: [
    MandateRoleTypeDetailComponent,
    MandateRoleTypeListComponent,
    MandateRoleTypeOverviewComponent,
    MandateRoleTypeFilterComponent
  ],
  exports: [
    MandateRoleTypesRoutingModule
  ]
})
export class AdministrationMandateRoleTypesModule { }
