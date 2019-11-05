import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ManagementService } from './management.service';

@NgModule({
  declarations: [
  ],
  providers: [
    ManagementService
  ],
  exports: [
  ]
})
export class ManagementServiceModule { }
