import { SortOrder } from './sort-order.enum';
import { Headers } from '@angular/http';

export class PagedResult<T> {
  public get data(): T[] { return this._data; }
  public get page(): number { return this._page; }
  public get pageSize(): number { return this._pageSize; }
  public get totalItems(): number { return this._totalItems; }
  public get totalPages(): number { return this._totalPages; }
  public get sortBy(): string { return this._sortBy; }
  public get sortOrder(): SortOrder { return this._sortOrder; }

  constructor(
    private _data: T[] = [],
    private _page: number = 1,
    private _pageSize: number = 10, // TODO: Get from config
    private _totalItems: number = 10,
    private _totalPages: number = 1,
    private _sortBy: string = '',
    private _sortOrder: SortOrder = SortOrder.Ascending) { }
}

// TODO: Correct naming?
export class PagedResultFactory<T> {
  public create(httpHeaders: Headers, data: T[]): PagedResult<T> {
    // {"currentPage":1,"itemsPerPage":2,"totalItems":25,"totalPages":13}
    let pagination = JSON.parse(httpHeaders.get('x-pagination')) || {};

    // {"sortBy":"vimid","sortOrder":"ascending"}
    let sorting = JSON.parse(httpHeaders.get('x-sorting')) || {};

    return new PagedResult<T>(
      data as T[] || [],
      pagination.currentPage,
      pagination.itemsPerPage,
      pagination.totalItems,
      pagination.totalPages,
      sorting.sortBy,
      sorting.sortOrder === 'ascending' ? SortOrder.Ascending : SortOrder.Descending);
  }
}
