import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { PersonDetailComponent } from './detail';
import { PersonOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/people',
    component: PersonOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Personen'
    }
  },
  {
    path: 'administration/people/create',
    component: PersonDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Nieuwe persoon'
    }
  },
  {
    path: 'administration/people/:id',
    component: PersonDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Bewerken persoon'
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
export class PeopleRoutingModule { }
