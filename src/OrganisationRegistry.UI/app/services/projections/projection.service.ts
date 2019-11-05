import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

@Injectable()
export class ProjectionService  {
  private projectionsUrl = `${this.configurationService.apiUrl}/v1/projections`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getAllProjections(): Observable<string[]> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(this.projectionsUrl, { headers: headers })
      .map(res => res.json());
  }
}
