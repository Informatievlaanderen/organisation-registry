import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { ProjectionOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'system/projections',
    component: ProjectionOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Systeem - Projectie herbouwen',
      roles: [Role.Developer]
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
export class ProjectionsRoutingModule { }
