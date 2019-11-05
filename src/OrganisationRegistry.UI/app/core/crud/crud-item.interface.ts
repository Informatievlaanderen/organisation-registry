export interface NoParamConstructor<T> {
  new (): T;
}

export interface ICrudItem<T> {
  id: string;
  name: string;
}
