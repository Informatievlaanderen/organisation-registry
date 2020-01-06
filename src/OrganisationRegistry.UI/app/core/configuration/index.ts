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
    return (<any>window).organisationRegistryVersion || '2.0.0.0';
  }

  public get apiUrl() {
    return (<any>window).organisationRegistryApiEndpoint || 'https://api.organisatie.dev-basisregisters.vlaanderen:9003';
  }

  public get uiUrl() {
    return (<any>window).organisationRegistryUiEndpoint || 'https://organisatie.dev-basisregisters.vlaanderen:3000';
  }
}
