import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';
import { HeadersBuilder } from 'core/http';

import { ConfigurationService } from 'core/configuration';
import { ApiConfiguration } from './';


@Injectable()
export class ApiConfigurationService  {
  private apiConfigurationUrl = `${this.configurationService.apiUrl}/v1/status/configuration`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getAllToggles(): Observable<ApiConfiguration> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(this.apiConfigurationUrl, { headers: headers })
      .map(this.toConfiguration);
  }

  private toConfiguration(res: Response): ApiConfiguration {
    let body = res.json();
    return body || {} as ApiConfiguration;
  }
}
