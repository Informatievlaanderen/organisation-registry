import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { OrganisationClassificationOverviewComponent } from './overview';
import { OrganisationClassificationDetailComponent } from './detail';

const routes: Routes = [
  {
    path: 'report/organisationclassification-translations',
    component: OrganisationClassificationOverviewComponent,
    data: {
      title: 'Rapportering - Organisatieclassificaties'
    }
  },
  {
    path: 'report/organisationclassification-translations/:id',
    component: OrganisationClassificationDetailComponent,
    data: {
      title: 'Rapportering - Organisatieclassificaties - Organisatieclassificatie'
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
export class OrganisationClassificationsRoutingModule { }
