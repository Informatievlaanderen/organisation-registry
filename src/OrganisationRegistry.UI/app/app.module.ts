import { ApplicationRef, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule } from '@angular/router';
import { createInputTransfer, createNewHosts, removeNgStyles } from '@angularclass/hmr';

import { ENV_PROVIDERS } from './environment';

import { App } from './app.component';
import { NoContent } from './no-content';

import { CoreModule } from './core';
import { SharedModule } from './shared';
import { AppRoutingModule } from './app-routing.module';
import { AdministrationModule } from './administration';
import { ManageModule } from './manage';
import { OrganisationModule } from './organisation';
import { BodyModule } from './body';
import { PeopleModule } from './people';
import { SystemModule } from './system';
import { ApiHelpModule } from './api';
import { ReportModule } from './report';
import { SearchModule } from './search';
import { AuthModule } from './auth';

// import { RouteReuseStrategy } from '@angular/router';
// import { CustomReuseStrategy } from './shared/router/custom-reuse-strategy';

type StoreType = {
  restoreInputValues: () => void,
  disposeOldHosts: () => void
};

@NgModule({
  bootstrap: [App],
  imports: [
    BrowserModule,
    CoreModule,
    SharedModule,
    AdministrationModule,
    ManageModule,
    OrganisationModule,
    BodyModule,
    PeopleModule,
    SystemModule,
    ApiHelpModule,
    ReportModule,
    SearchModule,
    AuthModule,
    AppRoutingModule
  ],
  declarations: [
    App,
    NoContent
  ],
  providers: [
    ENV_PROVIDERS,
    // { provide: RouteReuseStrategy, useClass: CustomReuseStrategy }
  ]
})
export class AppModule {
  constructor(public appRef: ApplicationRef) { }

  hmrOnInit(store: StoreType) {
    if (!store) return;
    console.log('HMR store', JSON.stringify(store, null, 2));

    // set input values
    if ('restoreInputValues' in store) {
      let restoreInputValues = store.restoreInputValues;
      setTimeout(restoreInputValues);
    }

    this.appRef.tick();
    delete store.restoreInputValues;
  }

  hmrOnDestroy(store: StoreType) {
    const cmpLocation = this.appRef.components.map(cmp => cmp.location.nativeElement);
    // recreate root elements
    store.disposeOldHosts = createNewHosts(cmpLocation);
    // save input values
    store.restoreInputValues = createInputTransfer();
    // remove styles
    removeNgStyles();
  }

  hmrAfterDestroy(store: StoreType) {
    // display new elements
    store.disposeOldHosts();
    delete store.disposeOldHosts;
  }
}
