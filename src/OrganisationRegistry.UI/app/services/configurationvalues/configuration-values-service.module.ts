import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ConfigurationValueService } from './configuration-value.service';

@NgModule({
  declarations: [
  ],
  providers: [
    ConfigurationValueService,
  ],
  exports: [
  ]
})
export class ConfigurationValuesServiceModule { }
