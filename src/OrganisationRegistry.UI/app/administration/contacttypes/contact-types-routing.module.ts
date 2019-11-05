import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { ContactTypeDetailComponent } from './detail';
import { ContactTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/contacttypes',
    component: ContactTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Contact types'
    }
  },
  {
    path: 'administration/contacttypes/create',
    component: ContactTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuw contact type'
    }
  },
  {
    path: 'administration/contacttypes/:id',
    component: ContactTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken contact type'
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
export class ContactTypesRoutingModule { }
