import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { OrganisationOverviewComponent } from './overview';
import { CreateOrganisationComponent } from './create';

const routes: Routes = [
  {
    path: 'organisations',
    component: OrganisationOverviewComponent,
    data: {
      title: 'Organisaties'
    }
  },
  {
    path: 'organisations/create',
    canActivate: [RoleGuard],
    canActivateChild: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.Developer],
      title: 'Nieuwe organisatie'
    },
    component: CreateOrganisationComponent
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class OrganisationRoutingModule { }
