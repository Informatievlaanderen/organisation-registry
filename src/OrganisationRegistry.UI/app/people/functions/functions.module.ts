import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { PeopleFunctionsServiceModule } from 'services/peoplefunctions';

import { PeopleFunctionsRoutingModule } from './functions-routing.module';

import { PeopleFunctionsComponent } from './functions.component';
import { PeopleFunctionsOverviewComponent } from './overview';
import { PeopleFunctionsListComponent } from './list';

@NgModule({
  imports: [
    SharedModule,
    PeopleFunctionsRoutingModule,
    PeopleFunctionsServiceModule
  ],
  declarations: [
    PeopleFunctionsComponent,
    PeopleFunctionsOverviewComponent,
    PeopleFunctionsListComponent,
  ],
  exports: [
    PeopleFunctionsRoutingModule
  ]
})
export class PeopleFunctionsModule { }
