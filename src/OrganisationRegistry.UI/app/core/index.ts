import { ModuleWithProviders, NgModule, Optional, SkipSelf } from '@angular/core';
import { Router, Routes, RouterModule, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpModule, JsonpModule, Http, XHRBackend, RequestOptions, Jsonp, JSONPBackend } from '@angular/http';

import {
  AuthService,
  OrganisationGuard,
  BodyGuard,
  RoleGuard,
  HttpInterceptor,
  JsonpInterceptor,
  OidcService,
  RolesResolver,
  ToggleGuard
} from './auth';

import { AlertComponent, AlertService } from './alert';
import { ConfigurationService } from './configuration';
import { FileSaverService } from './file-saver';
import { Constants } from './constants';

export function httpInterceptor(
  backend: XHRBackend,
  defaultOptions: RequestOptions,
  router: Router,
  activatedRoute: ActivatedRoute,
  configuration: ConfigurationService,
  oidcService: OidcService) {
  return new HttpInterceptor(backend, defaultOptions, router, activatedRoute, configuration, oidcService);
}

export function jsonpInterceptor(
  backend: JSONPBackend,
  defaultOptions: RequestOptions,
  alertService: AlertService) {
  return new JsonpInterceptor(backend, defaultOptions, alertService);
}

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    HttpModule,
    JsonpModule
  ],
  declarations: [
    AlertComponent
  ],
  providers: [
    Constants,
    AuthService,
    OidcService,
    OrganisationGuard,
    BodyGuard,
    RoleGuard,
    ToggleGuard,
    RolesResolver,
    AlertService,
    FileSaverService,
    ConfigurationService,
    {
      provide: Http,
      useFactory: httpInterceptor,
      deps: [ XHRBackend, RequestOptions, Router, ActivatedRoute, ConfigurationService, AlertService ]
    },
    {
      provide: Jsonp,
      useFactory: jsonpInterceptor,
      deps: [ JSONPBackend, RequestOptions, AlertService ]
    },

  ],
  exports: [
    AlertComponent
  ]
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error('CoreModule is already loaded. Import it in the AppModule only');
    }
  }
}
