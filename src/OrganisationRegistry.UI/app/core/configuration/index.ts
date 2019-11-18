import { Injectable } from '@angular/core';

@Injectable()
export class ConfigurationService {
  constructor() {
  }

  public get defaultPageSize() {
    return 10;
  }

  public get boundedPageSize() {
    return 500;
  }

  public get applicationVersion() {
    return (<any>window).wegwijsVersion || '2.0.0.0';
  }

  public get apiUrl() {
    return (<any>window).wegwijsApiEndpoint || 'https://api.wegwijs.dev.informatievlaanderen.be:8003';
  }

  public get authUrl() {
    return (<any>window).wegwijsAuthEndpoint || 'https://auth.wegwijs.dev.informatievlaanderen.be';
  }

  public get uiUrl() {
    return (<any>window).wegwijsUiEndpoint || 'http://wegwijs.dev.informatievlaanderen.be:3000';
  }
}
