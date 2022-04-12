import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { TerminatedInKboOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'system/terminated-in-kbo',
    component: TerminatedInKboOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      title: 'Systeem - Stopgezet in de KBO',
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
export class TerminatedInKboRoutingModule { }
