import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { CapacityOverviewComponent } from './overview';
import { CapacityDetailComponent } from './detail';

const routes: Routes = [
  {
    path: 'report/capacity-persons',
    component: CapacityOverviewComponent,
    data: {
      title: 'Rapportering - Hoedanigheden'
    }
  },
  {
    path: 'report/capacity-persons/:id',
    component: CapacityDetailComponent,
    data: {
      title: 'Rapportering - Hoedanigheden - Hoedanigheid'
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
export class CapacityPersonsRoutingModule { }
