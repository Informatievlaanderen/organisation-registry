import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { LabelTypeDetailComponent } from './detail';
import { LabelTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/labeltypes',
    component: LabelTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Benaming types'
    }
  },
  {
    path: 'administration/labeltypes/create',
    component: LabelTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Nieuw benaming type'
    }
  },
  {
    path: 'administration/labeltypes/:id',
    component: LabelTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Bewerken benaming type'
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
export class LabelTypesRoutingModule { }
