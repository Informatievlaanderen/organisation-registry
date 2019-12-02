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
    return (<any>window).wegwijsApiEndpoint || 'https://api.organisatie.staging-basisregisters.vlaanderen:9003';
  }

  public get authUrl() {
    return (<any>window).wegwijsAuthEndpoint || 'https://auth.organisatie.staging-basisregisters.vlaanderen';
  }

  public get uiUrl() {
    return (<any>window).wegwijsUiEndpoint || 'https://organisatie.staging-basisregisters.vlaanderen:3000';
  }
}
