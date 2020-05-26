import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ParticipationSummaryDetailComponent } from './detail';

const routes: Routes = [
  {
    path: 'report/participation-summary',
    component: ParticipationSummaryDetailComponent,
    data: {
      title: 'Rapportering - Participatie totaal'
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
export class ParticipationSummaryRoutingModule { }
