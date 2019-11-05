import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { FormalFrameworkOverviewComponent } from './overview';
import { FormalFrameworkDetailComponent } from './detail';

const routes: Routes = [
  {
    path: 'report/formalframework-bodies',
    component: FormalFrameworkOverviewComponent,
    data: {
      title: 'Rapportering - Toepassingsgebieden'
    }
  },
  {
    path: 'report/formalframework-bodies/:id',
    component: FormalFrameworkDetailComponent,
    data: {
      title: 'Rapportering - Toepassingsgebieden - Toepassingsgebied'
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
export class FormalFrameworkBodiesRoutingModule { }
