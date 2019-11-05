import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SharedModule } from 'shared';

import { RoleGuard, Role } from 'core/auth';

import { ApiConfigurationOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'system/api-configuration',
    component: ApiConfigurationOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Systeem - Api Configuratie',
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
export class ApiConfigurationRoutingModule { }
