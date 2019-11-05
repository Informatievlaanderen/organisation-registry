import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TogglesService } from './toggles.service';

@NgModule({
  declarations: [
  ],
  providers: [
    TogglesService
  ],
  exports: [
  ]
})
export class TogglesServiceModule { }
