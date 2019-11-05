import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { LifecyclePhaseTypeDetailComponent } from './detail';
import { LifecyclePhaseTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/lifecyclephasetypes',
    component: LifecyclePhaseTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Levensloopfase types'
    }
  },
  {
    path: 'administration/lifecyclephasetypes/create',
    component: LifecyclePhaseTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuw levensloopfase type'
    }
  },
  {
    path: 'administration/lifecyclephasetypes/:id',
    component: LifecyclePhaseTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken levensloopfase type'
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
export class LifecyclePhaseTypesRoutingModule { }
