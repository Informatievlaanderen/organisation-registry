import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { OrganisationRelationTypeDetailComponent } from './detail';
import { OrganisationRelationTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/organisationrelationtypes',
    component: OrganisationRelationTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Organisatie relatie types'
    }
  },
  {
    path: 'administration/organisationrelationtypes/create',
    component: OrganisationRelationTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Nieuw organisatie relatie type'
    }
  },
  {
    path: 'administration/organisationrelationtypes/:id',
    component: OrganisationRelationTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Bewerken organisatie relatie type'
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
export class OrganisationRelationTypesRoutingModule { }
