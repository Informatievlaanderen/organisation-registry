import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { OrganisationClassificationTypeDetailComponent } from './detail';
import { OrganisationClassificationTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/organisationclassificationtypes',
    component: OrganisationClassificationTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Organisatie classificatietypes'
    }
  },
  {
    path: 'administration/organisationclassificationtypes/create',
    component: OrganisationClassificationTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuw organisatie classificatietype'
    }
  },
  {
    path: 'administration/organisationclassificationtypes/:id',
    component: OrganisationClassificationTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken organisatie classificatietype'
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
export class OrganisationClassificationTypesRoutingModule { }
