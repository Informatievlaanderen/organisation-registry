import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { PurposeDetailComponent } from './detail';
import { PurposeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/purposes',
    component: PurposeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Beleidsvelden'
    }
  },
  {
    path: 'administration/purposes/create',
    component: PurposeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuw beleidsveld'
    }
  },
  {
    path: 'administration/purposes/:id',
    component: PurposeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken beleidsveld'
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
export class PurposesRoutingModule { }
