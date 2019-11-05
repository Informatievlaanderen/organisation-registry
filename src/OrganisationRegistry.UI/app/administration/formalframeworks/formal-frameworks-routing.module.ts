import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { FormalFrameworkDetailComponent } from './detail';
import { FormalFrameworkOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/formalframeworks',
    component: FormalFrameworkOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Toepassingsgebieden'
    }
  },
  {
    path: 'administration/formalframeworks/create',
    component: FormalFrameworkDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuw toepassingsgebied'
    }
  },
  {
    path: 'administration/formalframeworks/:id',
    component: FormalFrameworkDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken toepassingsgebied'
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
export class FormalFrameworksRoutingModule { }
