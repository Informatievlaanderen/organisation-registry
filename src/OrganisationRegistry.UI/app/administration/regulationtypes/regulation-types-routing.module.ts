import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { RegulationTypeDetailComponent } from './detail';
import { RegulationTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/regulationtypes',
    component: RegulationTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Regelgevingstypes'
    }
  },
  {
    path: 'administration/regulationtypes/create',
    component: RegulationTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuw regelgevingstype'
    }
  },
  {
    path: 'administration/regulationtypes/:id',
    component: RegulationTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken regelgevingstype'
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
export class RegulationTypesRoutingModule { }
