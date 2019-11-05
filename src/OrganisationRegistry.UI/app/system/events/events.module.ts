import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { EventsServiceModule } from 'services/events';
import { EventsRoutingModule } from './events-routing.module';

import { EventDataDetailComponent } from './detail';
import { EventDataOverviewComponent } from './overview';

import {
  EventDataListComponent,
  EventDataFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    EventsRoutingModule,
    EventsServiceModule
  ],
  declarations: [
    EventDataDetailComponent,
    EventDataListComponent,
    EventDataOverviewComponent,
    EventDataFilterComponent
  ],
  exports: [
    EventsRoutingModule
  ]
})
export class SystemEventsModule { }
