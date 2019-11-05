import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { ConfigurationValueDetailComponent } from './detail';
import { ConfigurationValueOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'system/configurationvalues',
    component: ConfigurationValueOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.Developer],
      title: 'Parameters - Configuratiewaarden'
    }
  },
  {
    path: 'system/configurationvalues/create',
    component: ConfigurationValueDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.Developer],
      title: 'Parameters - Nieuwe configuratiewaarde'
    }
  },
  {
    path: 'system/configurationvalues/:id',
    component: ConfigurationValueDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.Developer],
      title: 'Parameters - Bewerken configuratiewaarde'
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
export class ConfigurationValuesRoutingModule { }
