import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PeopleDetailComponent } from './detail.component'

import {
    PeopleInfoComponent,
    PeopleInfoOverviewComponent
} from 'people/info';

import {
    PeopleFunctionsComponent,
    PeopleFunctionsOverviewComponent
} from 'people/functions';

import {
    PeopleMandatesComponent,
    PeopleMandatesOverviewComponent
} from 'people/mandates';

import {
    PeopleCapacitiesComponent,
    PeopleCapacitiesOverviewComponent
} from 'people/capacities';

const routes: Routes = [
  {
    path: 'people/:id',
    component: PeopleDetailComponent,
    children: [
      { path: '', pathMatch: 'prefix', redirectTo: 'functions' },
      {
        path: 'info',
        component: PeopleInfoComponent,
        children: [
          {
            path: '',
            component: PeopleInfoOverviewComponent,
            data: { title: 'Persoon - Algemeen' }
          },
        ]
      },
      {
        path: 'functions',
        component: PeopleFunctionsComponent,
        children: [
          {
            path: '',
            component: PeopleFunctionsOverviewComponent,
            data: { title: 'Persoon - Functies' }
          },
        ]
      },
      {
        path: 'capacities',
        component: PeopleCapacitiesComponent,
        children: [
          {
            path: '',
            component: PeopleCapacitiesOverviewComponent,
            data: { title: 'Persoon - Hoedanigheden' }
          },
        ]
      },
      {
        path: 'mandates',
        component: PeopleMandatesComponent,
        children: [
          {
            path: '',
            component: PeopleMandatesOverviewComponent,
            data: { title: 'Persoon - Mandaten' }
          },
        ]
      },
    ]
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
export class PeopleDetailRoutingModule { }
