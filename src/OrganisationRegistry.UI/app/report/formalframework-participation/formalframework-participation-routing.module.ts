import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { FormalFrameworkParticipationOverviewComponent } from './overview';
import { FormalFrameworkParticipationDetailComponent } from './detail';

const routes: Routes = [
  {
    path: 'report/formalframework-participation',
    component: FormalFrameworkParticipationOverviewComponent,
    data: {
      title: 'Rapportering - Participatie'
    }
  },
  {
    path: 'report/formalframework-participation/:id',
    component: FormalFrameworkParticipationDetailComponent,
    data: {
      title: 'Rapportering - Participatie - Orgaan'
    }
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class FormalFrameworkParticipationRoutingModule { }
