import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SharedModule } from 'shared';

import { RoleGuard, Role } from 'core/auth';

import { StatsOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'system/stats',
    component: StatsOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Systeem - Statistieken',
      roles: [ Role.Developer ]
    }
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes),
    SharedModule
  ],
  exports: [
    RouterModule
  ]
})
export class StatsRoutingModule { }
