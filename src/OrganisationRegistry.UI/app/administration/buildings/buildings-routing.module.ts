import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { BuildingDetailComponent } from './detail';
import { BuildingOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/buildings',
    component: BuildingOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Gebouwen'
    }
  },
  {
    path: 'administration/buildings/create',
    component: BuildingDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Nieuw gebouw'
    }
  },
  {
    path: 'administration/buildings/:id',
    component: BuildingDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Bewerken gebouw'
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
export class BuildingsRoutingModule { }
