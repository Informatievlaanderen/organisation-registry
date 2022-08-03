import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { DashboardOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'manage',
    component: DashboardOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder,Role.CjmBeheerder, Role.DecentraalBeheerder],
      title: 'Beheer'
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
export class DashboardRoutingModule { }
