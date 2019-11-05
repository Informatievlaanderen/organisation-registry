import { Injectable } from '@angular/core';
import { Response, Headers, Http } from '@angular/http';

import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from 'core/configuration';
import { HeadersBuilder } from 'core/http';

import {
  Exception
} from './';

@Injectable()
export class ExceptionsService  {
  private exceptionsUrl = `${this.configurationService.apiUrl}/v1/status/exceptions`;

  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) { }

  public getAllExceptions(): Observable<Exception[]> {
    let headers = new HeadersBuilder()
      .json()
      .build();

    return this.http
      .get(this.exceptionsUrl, { headers: headers })
      .map(this.toExceptions);
  }

  private toExceptions(res: Response): Exception[] {
    let exceptions = res.json();
    return exceptions.map(exc => new Exception(exc[0],exc[1],exc[2],exc[3]));
  }
}
