import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { PeopleInfoServiceModule } from 'services';

import { PeopleInfoModule } from 'people/info';

import { PeopleFunctionsModule } from 'people/functions';
import { PeopleCapacitiesModule } from 'people/capacities';
import { PeopleMandatesModule } from 'people/mandates';

import { PeopleDetailRoutingModule } from './detail-routing.module';
import { PeopleDetailComponent } from './detail.component';

@NgModule({
  imports: [
    SharedModule,
    PeopleDetailRoutingModule,

    PeopleInfoModule,
    PeopleInfoServiceModule,

    PeopleFunctionsModule,
    PeopleCapacitiesModule,
    PeopleMandatesModule
  ],
  declarations: [
    PeopleDetailComponent,
  ],
  exports: [
    PeopleDetailRoutingModule
  ]
})
export class PeopleDetailModule { }
