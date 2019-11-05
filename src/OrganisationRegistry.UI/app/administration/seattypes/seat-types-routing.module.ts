import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { SeatTypeDetailComponent } from './detail';
import { SeatTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/seattypes',
    component: SeatTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Post types'
    }
  },
  {
    path: 'administration/seattypes/create',
    component: SeatTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuw post type'
    }
  },
  {
    path: 'administration/seattypes/:id',
    component: SeatTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken post type'
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
export class SeatTypesRoutingModule { }
