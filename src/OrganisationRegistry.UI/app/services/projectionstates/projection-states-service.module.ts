import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ProjectionStateService } from './projection-state.service';

@NgModule({
  declarations: [
  ],
  providers: [
    ProjectionStateService
  ],
  exports: [
  ]
})
export class ProjectionStatesServiceModule { }
