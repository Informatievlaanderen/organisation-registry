import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DelegationService } from './delegation.service';

@NgModule({
  declarations: [
  ],
  providers: [
    DelegationService
  ],
  exports: [
  ]
})
export class DelegationsServiceModule { }
