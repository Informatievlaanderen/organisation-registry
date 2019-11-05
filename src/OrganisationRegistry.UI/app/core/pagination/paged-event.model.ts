import { SortOrder } from './sort-order.enum';

export class PagedEvent {
  public get page(): number { return this._page; }
  public get pageSize(): number { return this._pageSize; }
  public get sortBy(): string { return this._sortBy; }
  public get sortOrder(): SortOrder { return this._sortOrder; }

  constructor(
    private _page: number,
    private _pageSize: number,
    private _sortBy: string,
    private _sortOrder: SortOrder) { }
}
