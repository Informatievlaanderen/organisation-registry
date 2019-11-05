import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LifecyclePhaseTypeService } from './lifecycle-phase-type.service';
import { LifecyclePhaseTypeResolver } from './lifecycle-phase-type.resolver';

@NgModule({
  declarations: [
  ],
  providers: [
    LifecyclePhaseTypeService,
    LifecyclePhaseTypeResolver
  ],
  exports: [
  ]
})
export class LifecyclePhaseTypesServiceModule { }
