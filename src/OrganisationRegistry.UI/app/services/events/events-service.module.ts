import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { EventService } from './event.service';

@NgModule({
  declarations: [
  ],
  providers: [
    EventService
  ],
  exports: [
  ]
})
export class EventsServiceModule { }
