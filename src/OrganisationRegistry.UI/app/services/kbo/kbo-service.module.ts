import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { KboService } from './kbo.service';

@NgModule({
  declarations: [
  ],
  providers: [
    KboService,
  ],
  exports: [
  ]
})
export class KboServiceModule { }
