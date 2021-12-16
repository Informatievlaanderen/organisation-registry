import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationVlimpersRoutingModule } from './vlimpers-routing.module';

import { OrganisationVlimpersComponent } from './vlimpers.component';
import { OrganisationVlimpersOverviewComponent } from './overview';
import {OrganisationVlimpersServiceModule} from "services/organisationvlimpers";


@NgModule({
  imports: [
    SharedModule,
    OrganisationVlimpersRoutingModule,
    OrganisationVlimpersServiceModule,
  ],
  declarations: [
    OrganisationVlimpersComponent,
    OrganisationVlimpersOverviewComponent,
  ],
  exports: [
    OrganisationVlimpersRoutingModule
  ]
})
export class OrganisationVlimpersModule { }
