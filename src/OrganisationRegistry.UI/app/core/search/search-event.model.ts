export class SearchEvent<T> {
  public get fields(): T { return this._fields; }

  constructor(private _fields: T) { }
}
