import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { BodyLifecyclePhaseService } from './body-lifecycle-phase.service';

@NgModule({
  declarations: [
  ],
  providers: [
    BodyLifecyclePhaseService
  ],
  exports: [
  ]
})
export class BodyLifecyclePhasesServiceModule { }
