import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodiesServiceModule } from 'services/bodies';
import { BodyMandatesServiceModule } from 'services/bodymandates';

import { BodyMandatesRoutingModule } from './mandates-routing.module';

import { BodyMandatesComponent } from './mandates.component';
import { BodyMandatesOverviewComponent } from './overview';
import { BodyMandatesListComponent } from './list';
import { BodyMandatesFilterComponent } from './filter';
import { BodyMandatesLinkPersonComponent } from './linkperson';
import { BodyMandatesLinkFunctionComponent } from './linkfunction';
import { BodyMandatesLinkOrganisationComponent } from './linkorganisation';
import { BodyMandatesUpdatePersonComponent } from './updateperson';
import { BodyMandatesUpdateFunctionComponent } from './updatefunction';
import { BodyMandatesUpdateOrganisationComponent } from './updateorganisation';
@NgModule({
  imports: [
    SharedModule,
    BodyMandatesRoutingModule,
    BodyMandatesServiceModule,
    BodiesServiceModule
  ],
  declarations: [
    BodyMandatesComponent,
    BodyMandatesOverviewComponent,
    BodyMandatesListComponent,
    BodyMandatesFilterComponent,
    BodyMandatesLinkPersonComponent,
    BodyMandatesLinkFunctionComponent,
    BodyMandatesLinkOrganisationComponent,
    BodyMandatesUpdatePersonComponent,
    BodyMandatesUpdateFunctionComponent,
    BodyMandatesUpdateOrganisationComponent
  ],
  exports: [
    BodyMandatesRoutingModule
  ]
})
export class BodyMandatesModule { }
