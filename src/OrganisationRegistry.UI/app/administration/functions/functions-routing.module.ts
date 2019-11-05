import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { FunctionDetailComponent } from './detail';
import { FunctionOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/functions',
    component: FunctionOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Functies'
    }
  },
  {
    path: 'administration/functions/create',
    component: FunctionDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuwe functie'
    }
  },
  {
    path: 'administration/functions/:id',
    component: FunctionDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken functie'
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
export class FunctionsRoutingModule { }
