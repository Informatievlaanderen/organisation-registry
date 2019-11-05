import { Observable } from 'rxjs/Observable';

import { NoParamConstructor } from './crud-item.interface';

export interface ICrud<T> {
  load(ctor: NoParamConstructor<T>): Observable<T>;

  save(item: T): Observable<string>;

  alertLoadError(error: any): void;

  alertSaveSuccess(model: T, itemUrl: string): void;

  alertSaveError(error: any): void;
}
