import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { MandateRoleTypeDetailComponent } from './detail';
import { MandateRoleTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/mandateroletypes',
    component: MandateRoleTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Mandaat rollen'
    }
  },
  {
    path: 'administration/mandateroletypes/create',
    component: MandateRoleTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Nieuwe mandaat rol'
    }
  },
  {
    path: 'administration/mandateroletypes/:id',
    component: MandateRoleTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Bewerken mandaat rol'
    }
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class MandateRoleTypesRoutingModule { }
