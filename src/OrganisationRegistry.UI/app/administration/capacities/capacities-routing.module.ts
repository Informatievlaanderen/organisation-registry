import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { CapacityDetailComponent } from './detail';
import { CapacityOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/capacities',
    component: CapacityOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Hoedanigheden'
    }
  },
  {
    path: 'administration/capacities/create',
    component: CapacityDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuwe hoedanigheid'
    }
  },
  {
    path: 'administration/capacities/:id',
    component: CapacityDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Bewerken hoedanigheid'
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
export class CapacityPersonsRoutingModule { }
