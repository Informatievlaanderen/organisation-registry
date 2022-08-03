import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { LocationDetailComponent } from './detail';
import { LocationOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/locations',
    component: LocationOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Locaties'
    }
  },
  {
    path: 'administration/locations/create',
    component: LocationDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Nieuwe locatie'
    }
  },
  {
    path: 'administration/locations/:id',
    component: LocationDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Bewerken locatie'
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
export class LocationsRoutingModule { }
