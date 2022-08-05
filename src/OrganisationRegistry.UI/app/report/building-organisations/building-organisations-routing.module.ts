import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { BuildingOverviewComponent } from './overview';
import { BuildingDetailComponent } from './detail';

const routes: Routes = [
  {
    path: 'report/building-organisations',
    component: BuildingOverviewComponent,
    data: {
      title: 'Rapportering - Gebouwen'
    }
  },
  {
    path: 'report/building-organisations/:id',
    component: BuildingDetailComponent,
    data: {
      title: 'Rapportering - Gebouwen - Gebouw'
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
export class BuildingOrganisationsRoutingModule { }
