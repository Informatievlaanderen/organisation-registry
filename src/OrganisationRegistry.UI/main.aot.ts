import { platformBrowser } from '@angular/platform-browser';
import { AppModuleNgFactory } from './../../aot/src/OrganisationRegistry.UI/app/app.module.ngfactory';

platformBrowser()
  .bootstrapModuleFactory(AppModuleNgFactory)
  .catch(err => console.error(err));
