import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { RegulationThemeDetailComponent } from './detail';
import { RegulationThemeOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/regulation-themes',
    component: RegulationThemeOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Regelgevingthema\'s'
    }
  },
  {
    path: 'administration/regulation-themes/create',
    component: RegulationThemeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Nieuw regelgevingsthema'
    }
  },
  {
    path: 'administration/regulation-themes/:id',
    component: RegulationThemeDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.OrganisationRegistryBeheerder],
      title: 'Parameters - Bewerken regelgevingsthema'
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
export class RegulationThemesRoutingModule { }
