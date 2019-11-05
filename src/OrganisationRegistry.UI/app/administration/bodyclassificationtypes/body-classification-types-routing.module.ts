import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { BodyClassificationTypeDetailComponent } from './detail';
import { BodyClassificationTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/bodyclassificationtypes',
    component: BodyClassificationTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Orgaan classificatietypes'
    }
  },
  {
    path: 'administration/bodyclassificationtypes/create',
    component: BodyClassificationTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuw orgaan classificatietype'
    }
  },
  {
    path: 'administration/bodyclassificationtypes/:id',
    component: BodyClassificationTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken orgaan classificatietype'
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
export class BodyClassificationTypesRoutingModule { }
