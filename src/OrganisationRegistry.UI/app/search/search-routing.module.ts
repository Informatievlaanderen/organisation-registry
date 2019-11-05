import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { BlankSearchComponent } from './blank';
import { BodySearchComponent } from './bodies';
import { OrganisationSearchComponent } from './organisations';
import { PersonSearchComponent } from './people';

const routes: Routes = [
  {
    path: 'search/blank',
    component: BlankSearchComponent
  },
  {
    path: 'search/bodies',
    component: BodySearchComponent,
    data: {
      title: 'Organen zoeken'
    }
  },
  {
    path: 'search/organisations',
    component: OrganisationSearchComponent,
    data: {
      title: 'Organisaties zoeken'
    }
  },
  {
    path: 'search/people',
    component: PersonSearchComponent,
    data: {
      title: 'Personen zoeken'
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
export class SearchRoutingModule { }
