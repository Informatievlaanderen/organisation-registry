import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { KeyTypeDetailComponent } from './detail';
import { KeyTypeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/keytypes',
    component: KeyTypeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Informatiesystemen'
    }
  },
  {
    path: 'administration/keytypes/create',
    component: KeyTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Nieuw informatiesysteem'
    }
  },
  {
    path: 'administration/keytypes/:id',
    component: KeyTypeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Bewerken informatiesysteem'
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
export class KeyTypesRoutingModule { }
