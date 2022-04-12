import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { DelegationDetailComponent } from './detail';
import { DelegationOverviewComponent } from './overview';
import { DelegationAssignimentsCreateDelegationAssignmentComponent } from './create';
import { DelegationAssignimentsUpdateDelegationAssignmentComponent } from './update';
import { DelegationAssignimentsDeleteDelegationAssignmentComponent } from './delete';

import { ContactTypeResolver } from 'services/contacttypes';

const routes: Routes = [
  {
    path: 'manage/delegations',
    component: DelegationOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.DecentraalBeheerder],
      title: 'Delegaties'
    }
  },
  {
    path: 'manage/delegations/:id',
    component: DelegationDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.DecentraalBeheerder],
      title: 'Delegaties - Bewerken delegatie'
    }
  },
  {
    path: 'manage/delegations/:id/create',
    component: DelegationAssignimentsCreateDelegationAssignmentComponent,
    canActivate: [RoleGuard],
    resolve: {
      contactTypes: ContactTypeResolver
    },
    data: {
      roles: [Role.AlgemeenBeheerder, Role.DecentraalBeheerder],
      title: 'Delegaties - Toewijzen delegatie'
    }
  },
  {
    path: 'manage/delegations/:bodyMandateId/edit/:id',
    component: DelegationAssignimentsUpdateDelegationAssignmentComponent,
    canActivate: [RoleGuard],
    resolve: {
      contactTypes: ContactTypeResolver
    },
    data: {
      roles: [Role.AlgemeenBeheerder, Role.DecentraalBeheerder],
      title: 'Delegaties - Bewerken toewijzing'
    }
  },
  {
    path: 'manage/delegations/:bodyMandateId/delete/:id',
    component: DelegationAssignimentsDeleteDelegationAssignmentComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.DecentraalBeheerder],
      title: 'Delegaties - Verwijderen toewijzing'
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
export class DelegationsRoutingModule { }
