import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ExceptionsService } from './exceptions.service';

@NgModule({
  declarations: [
  ],
  providers: [
    ExceptionsService
  ],
  exports: [
  ]
})
export class ExceptionsServiceModule { }
