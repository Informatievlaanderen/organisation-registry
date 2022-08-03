import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { OrganisationClassificationDetailComponent } from './detail';
import { OrganisationClassificationOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/organisationclassifications',
    component: OrganisationClassificationOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Organisatieclassificaties'
    }
  },
  {
    path: 'administration/organisationclassifications/create',
    component: OrganisationClassificationDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Nieuwe organisatieclassificatie'
    }
  },
  {
    path: 'administration/organisationclassifications/:id',
    component: OrganisationClassificationDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Bewerken organisatieclassificatie'
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
export class OrganisationClassificationsRoutingModule { }
