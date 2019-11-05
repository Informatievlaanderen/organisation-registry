import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { FunctionService } from './function.service';
import { FunctionResolver } from './function.resolver';

@NgModule({
  declarations: [
  ],
  providers: [
    FunctionService,
    FunctionResolver
  ],
  exports: [
  ]
})
export class FunctionsServiceModule { }
