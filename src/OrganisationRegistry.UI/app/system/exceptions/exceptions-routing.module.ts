import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SharedModule } from 'shared';

import { RoleGuard, Role } from 'core/auth';

import { ExceptionOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'system/exceptions',
    component: ExceptionOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Systeem - Foutmeldingen',
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
export class ExceptionsRoutingModule { }
