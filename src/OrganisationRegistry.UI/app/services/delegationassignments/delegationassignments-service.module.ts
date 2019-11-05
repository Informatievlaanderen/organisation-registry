import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DelegationAssignmentService } from './delegationassignment.service';

@NgModule({
  declarations: [
  ],
  providers: [
    DelegationAssignmentService
  ],
  exports: [
  ]
})
export class DelegationAssignmentsServiceModule { }
