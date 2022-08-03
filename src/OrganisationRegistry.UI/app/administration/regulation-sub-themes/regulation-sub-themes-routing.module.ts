import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { RegulationSubThemeDetailComponent } from './detail';
import { RegulationSubThemeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/regulation-sub-themes',
    component: RegulationSubThemeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Regelgevingsubthema\'s'
    }
  },
  {
    path: 'administration/regulation-sub-themes/create',
    component: RegulationSubThemeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Nieuw Regelgevingsubthema'
    }
  },
  {
    path: 'administration/regulation-sub-themes/:id',
    component: RegulationSubThemeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
      title: 'Parameters - Bewerken Regelgevingsubthema'
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
export class RegulationSubThemesRoutingModule { }
