import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { FormalFrameworkCategoriesServiceModule } from 'services/formalframeworkcategories';
import { FormalFrameworkCategoriesRoutingModule } from './formal-framework-categories-routing.module';

import { FormalFrameworkCategoryDetailComponent } from './detail';
import { FormalFrameworkCategoryOverviewComponent } from './overview';

import {
  FormalFrameworkCategoryListComponent,
  FormalFrameworkCategoryFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    FormalFrameworkCategoriesRoutingModule,
    FormalFrameworkCategoriesServiceModule
  ],
  declarations: [
    FormalFrameworkCategoryDetailComponent,
    FormalFrameworkCategoryListComponent,
    FormalFrameworkCategoryOverviewComponent,
    FormalFrameworkCategoryFilterComponent
  ],
  exports: [
    FormalFrameworkCategoriesRoutingModule
  ]
})
export class AdministrationFormalFrameworkCategoriesModule { }
