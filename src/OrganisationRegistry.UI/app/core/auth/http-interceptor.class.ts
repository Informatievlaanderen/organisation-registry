import {
  ConnectionBackend,
  Headers,
  Http,
  Request,
  RequestOptionsArgs,
  Response,
  RequestOptions
} from '@angular/http';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { ConfigurationService } from './../configuration';
import {OidcService} from "./oidc.service";

export class HttpInterceptor extends Http {
  private securityUrl = `${this.configurationService.apiUrl}/v1/security`;

  constructor(
    backend: ConnectionBackend,
    defaultOptions: RequestOptions,
    private router: Router,
    private route: ActivatedRoute,
    private configurationService: ConfigurationService,
    private oidcService: OidcService
  ) {
    super(backend, defaultOptions);
  }

  request(url: string | Request, options?: RequestOptionsArgs): Observable<Response> {
    let requestedUrl = (url instanceof Request) ? url.url : url;
    if (requestedUrl === this.securityUrl)
      return super.request(url, this.getRequestOptionArgs(options));

    // console.log('http intercept - request', url);
    return this.intercept(super.request(url, this.getRequestOptionArgs(options)));
  }

  get(url: string, options?: RequestOptionsArgs): Observable<Response> {
    if (url === this.securityUrl)
      return super.get(url, this.getRequestOptionArgs(options));

    // console.log('http intercept - get', url);
    return this.intercept(super.get(url, this.getRequestOptionArgs(options)));
  }

  post(url: string, body: string, options?: RequestOptionsArgs): Observable<Response> {
    if (url === this.securityUrl)
      return super.post(url, body, this.getRequestOptionArgs(options));

    // console.log('http intercept - post', url);
    return this.intercept(super.post(url, body, this.getRequestOptionArgs(options)));
  }

  put(url: string, body: string, options?: RequestOptionsArgs): Observable<Response> {
    if (url === this.securityUrl)
      return super.put(url, body, this.getRequestOptionArgs(options));

    // console.log('http intercept - put', url);
    return this.intercept(super.put(url, body, this.getRequestOptionArgs(options)));
  }

  delete(url: string, options?: RequestOptionsArgs): Observable<Response> {
    if (url === this.securityUrl)
      return super.delete(url, this.getRequestOptionArgs(options));

    // console.log('http intercept - delete', url);
    return this.intercept(super.delete(url, this.getRequestOptionArgs(options)));
  }

  getRequestOptionArgs(options?: RequestOptionsArgs): RequestOptionsArgs {
    if (options == null)
      options = new RequestOptions();

    if (options.headers == null)
      options.headers = new Headers();

    options.headers.append('content-type', 'application/json');
    options.withCredentials = true;
    return options;
  }

  intercept(observable: Observable<Response>): Observable<Response> {
    return observable.catch((err, source) => {
      switch (err.status) {
        case 401:
          this.oidcService.signIn();
          return Observable.empty();
        case 403:
          break;
        case 404:
          this.router.navigate(['404']);
          return Observable.empty();
        default:
          let error = err.json();
          if (error.reference && error.detail) {
            console.error(`${error.detail} Indien het probleem zich blijft voordien, contacteer uw beheerder met referentie ${error.reference}.`);
          }
          return Observable.throw(err);
      }
    });
  }
}
