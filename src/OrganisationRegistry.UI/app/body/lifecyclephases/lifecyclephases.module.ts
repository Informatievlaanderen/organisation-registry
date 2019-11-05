import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodiesServiceModule } from 'services/bodies';

import { BodyLifecyclePhasesServiceModule } from 'services/bodylifecyclephases';
import { BodyLifecyclePhasesOverviewComponent } from './overview';
import { BodyLifecyclePhasesListComponent } from './list';
import { BodyLifecyclePhasesComponent } from './lifecyclephases.component';
import { BodyLifecyclePhasesRoutingModule } from './lifecyclephases-routing.module';
import { BodyLifecyclePhasesCreateBodyLifecyclePhaseComponent } from './create';
import { BodyLifecyclePhasesUpdateBodyLifecyclePhaseComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    BodyLifecyclePhasesRoutingModule,
    BodyLifecyclePhasesServiceModule,
    BodiesServiceModule
  ],
  declarations: [
    BodyLifecyclePhasesComponent,
    BodyLifecyclePhasesOverviewComponent,
    BodyLifecyclePhasesListComponent,
    BodyLifecyclePhasesCreateBodyLifecyclePhaseComponent,
    BodyLifecyclePhasesUpdateBodyLifecyclePhaseComponent
  ],
  exports: [
    BodyLifecyclePhasesRoutingModule
  ]
})
export class BodyLifecyclePhasesModule { }
