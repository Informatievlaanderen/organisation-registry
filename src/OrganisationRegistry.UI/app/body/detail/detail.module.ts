import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodyInfoServiceModule } from 'services';

import { BodyInfoModule } from 'body/info';
import { BodyContactsModule } from 'body/contacts';
import { BodyOrganisationsModule } from 'body/organisations';
import { BodyCompositionModule } from 'body/composition';
import { BodyLifecyclePhasesModule } from 'body/lifecyclephases';
import { BodyMandatesModule } from 'body/mandates';
import { BodyFormalFrameworksModule } from 'body/formalframeworks';
import { BodyParticipationModule } from 'body/participation';
import { BodyResponsibilitiesModule } from 'body/responsibilities';
import { BodyClassificationsModule } from 'body/classifications';

import { BodyDetailRoutingModule } from './detail-routing.module';
import { BodyDetailComponent } from './detail.component';

@NgModule({
  imports: [
    SharedModule,
    BodyDetailRoutingModule,

    BodyInfoModule,
    BodyInfoServiceModule,

    BodyContactsModule,
    BodyOrganisationsModule,
    BodyCompositionModule,
    BodyLifecyclePhasesModule,
    BodyMandatesModule,
    BodyFormalFrameworksModule,
    BodyParticipationModule,
    BodyResponsibilitiesModule,
    BodyClassificationsModule
  ],
  declarations: [
    BodyDetailComponent
  ],
  exports: [
    BodyDetailRoutingModule
  ]
})
export class BodyDetailModule { }
