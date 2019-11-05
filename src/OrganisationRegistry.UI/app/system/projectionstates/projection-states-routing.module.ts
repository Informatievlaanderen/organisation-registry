import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { ProjectionStateOverviewComponent } from './overview';
import { ProjectionStateDetailComponent } from './detail';

const routes: Routes = [
  {
    path: 'system/projection-states',
    component: ProjectionStateOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Systeem - Projectie status',
      roles: [Role.Developer]
    }
  },
  {
    path: 'system/projection-states/:id',
    component: ProjectionStateDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.Developer],
      title: 'Systeem - Projectie status - Bewerken status'
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
export class ProjectionStatesRoutingModule { }
