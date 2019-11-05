import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { PeopleInfoRoutingModule } from './info-routing.module';

import { PeopleInfoComponent } from './info.component';
import { PeopleInfoOverviewComponent } from './overview';

@NgModule({
  imports: [
    SharedModule,
    PeopleInfoRoutingModule,
  ],
  declarations: [
    PeopleInfoComponent,
    PeopleInfoOverviewComponent,
  ],
  exports: [
    PeopleInfoRoutingModule
  ]
})
export class PeopleInfoModule { }
