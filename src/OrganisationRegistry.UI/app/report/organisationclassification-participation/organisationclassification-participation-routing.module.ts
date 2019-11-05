import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { OrganisationClassificationParticipationOverviewComponent } from './overview';
import { OrganisationClassificationParticipationDetailComponent } from './detail';

const routes: Routes = [
  {
    path: 'report/organisationclassification-participation/:tag',
    component: OrganisationClassificationParticipationOverviewComponent,
    data: {
      title: 'Rapportering - Participatie'
    }
  },
  {
    path: 'report/organisationclassification-participation/:tag/:id',
    component: OrganisationClassificationParticipationDetailComponent,
    data: {
      title: 'Rapportering - Participatie - Beleidsdomein'
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
export class OrganisationClassificationParticipationRoutingModule { }
