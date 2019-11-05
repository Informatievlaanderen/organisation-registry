import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { StatsService } from './stats.service';

@NgModule({
  declarations: [
  ],
  providers: [
    StatsService
  ],
  exports: [
  ]
})
export class StatsServiceModule { }
