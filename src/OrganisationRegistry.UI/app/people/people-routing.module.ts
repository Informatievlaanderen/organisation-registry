import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PersonOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'people',
    component: PersonOverviewComponent,
    data: {
      title: 'Personen'
    }
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class PeopleRoutingModule { }
