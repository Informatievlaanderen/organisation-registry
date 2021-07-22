import { Injectable } from '@angular/core';
import { Response, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

import {
  Features
} from './';

@Injectable()
export class FeaturesService  {
  private featuresUrl = `${this.configurationService.apiUrl}/v1/status/features`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getAllFeatures(): Observable<Features> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(this.featuresUrl, { headers: headers })
      .map(FeaturesService.toFeatures);
  }

  private static toFeatures(res: Response): Features {
    let body = res.json();
    return body || {} as Features;
  }
}
