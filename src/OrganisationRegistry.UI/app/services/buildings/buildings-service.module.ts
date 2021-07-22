import { HttpModule } from '@angular/http';
import { NgModule } from '@angular/core';

import { CoreModule } from 'core';

import { BuildingService } from './building.service';

@NgModule({
  imports: [
    HttpModule,
    CoreModule
  ],
  declarations: [
  ],
  providers: [
    BuildingService
  ],
  exports: [
  ]
})
export class BuildingsServiceModule { }
