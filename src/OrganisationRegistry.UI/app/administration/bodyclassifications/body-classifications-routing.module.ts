import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { BodyClassificationDetailComponent } from './detail';
import { BodyClassificationOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/bodyclassifications',
    component: BodyClassificationOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Orgaanclassificaties'
    }
  },
  {
    path: 'administration/bodyclassifications/create',
    component: BodyClassificationDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Nieuwe orgaanclassificatie'
    }
  },
  {
    path: 'administration/bodyclassifications/:id',
    component: BodyClassificationDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Bewerken orgaanclassificatie'
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
export class BodyClassificationsRoutingModule { }
