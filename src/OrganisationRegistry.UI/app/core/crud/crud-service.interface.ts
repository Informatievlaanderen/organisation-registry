import { Observable } from 'rxjs/Observable';

export interface ICrudService<T> {
  get(itemId: string): Observable<T>;

  create(item: T): Observable<string>;

  update(item: T): Observable<string>;
}
