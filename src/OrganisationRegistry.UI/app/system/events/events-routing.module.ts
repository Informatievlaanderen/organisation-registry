import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { EventDataDetailComponent } from './detail';
import { EventDataOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'system/events',
    component: EventDataOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Systeem - Events',
      roles: [Role.AlgemeenBeheerder, Role.Developer]
    }
  },
  {
    path: 'system/events/:id',
    component: EventDataDetailComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Systeem - Event - Algemeen',
      roles: [Role.AlgemeenBeheerder, Role.Developer]
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
export class EventsRoutingModule { }
