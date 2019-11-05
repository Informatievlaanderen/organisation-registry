import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { DashboardOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'system',
    component: DashboardOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Systeem',
      roles: [Role.OrganisationRegistryBeheerder, Role.Developer]
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
