import {
  ConnectionBackend,
  Headers,
  Jsonp,
  Request,
  RequestOptionsArgs,
  Response,
  RequestOptions
} from '@angular/http';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder } from './../alert';
import { ConfigurationService } from './../configuration';

export class JsonpInterceptor extends Jsonp {
  constructor(
    backend: ConnectionBackend,
    defaultOptions: RequestOptions,
    private alertService: AlertService
  ) {
    super(backend, defaultOptions);
  }

  request(url: string | Request, options?: RequestOptionsArgs): Observable<Response> {
    return this.intercept(super.request(url, options));
  }

  intercept(observable: Observable<Response>): Observable<Response> {
    return observable.catch((err, source) => {
        return Observable.throw(err);
    });
  }
}
