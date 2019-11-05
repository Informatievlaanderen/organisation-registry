import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SharedModule } from 'shared';

import { RoleGuard, Role } from 'core/auth';

import { TogglesOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'system/toggles',
    component: TogglesOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Systeem - Toggles',
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
export class TogglesRoutingModule { }
