import { NgModule } from '@angular/core';

import { RegulationTypeService } from './regulation-type.service';
import { RegulationTypeResolver } from './regulation-type.resolver';

@NgModule({
  declarations: [
  ],
  providers: [
    RegulationTypeService,
    RegulationTypeResolver
  ],
  exports: [
  ]
})
export class RegulationTypesServiceModule { }
