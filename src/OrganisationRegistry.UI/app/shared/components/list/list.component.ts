import { Component, Input, Output, EventEmitter } from '@angular/core';

import { PagedResult, PagedEvent, SortOrder } from './../../../core/pagination';

@Component({
  inputs: ['items', 'isBusy'],
  outputs: ['changePage', 'exportCsv'],
  template: ''
})
export class BaseListComponent<T> {
  public items: PagedResult<T>;
  public isBusy: boolean = true;
  public changePage: EventEmitter<PagedEvent> = new EventEmitter<PagedEvent>();
  public exportCsv: EventEmitter<{}> = new EventEmitter();

  get data(): T[] {
    return this.items.data;
  }

  hasData(data: any[]): boolean {
    return data.length > 0;
  }

  get page(): number {
    return this.items.page;
  }

  get pageSize(): number {
    return this.items.pageSize;
  }

  get totalItems(): number {
    return this.items.totalItems;
  }

  get totalPages(): number {
    return this.items.totalPages;
  }

  get firstItem(): number {
    return ((this.page - 1) * this.pageSize) + 1;
  }

  get lastItem(): number {
    let lastItem = this.page * this.pageSize;
    return (lastItem > this.totalItems) ? this.totalItems : lastItem;
  }

  get sortedBy(): string {
    return this.items.sortBy;
  }

  get sortOrder(): SortOrder {
    return this.items.sortOrder;
  }

  isSortedBy(sortBy: string): boolean {
    return this.sortedBy.toUpperCase() === sortBy.toUpperCase();
  }

  isSortedByDescending(sortBy: string): boolean {
    return this.isSortedBy(sortBy) && this.sortOrder === SortOrder.Descending;
  }

  nextPage() {
    this.changePage.emit(new PagedEvent(
      this.page + 1,
      this.items.pageSize,
      this.sortedBy,
      this.sortOrder));
  }

  previousPage() {
    this.changePage.emit(new PagedEvent(
      this.page - 1,
      this.items.pageSize,
      this.sortedBy,
      this.sortOrder));
  }

  sortBy(sortBy: string) {
    // If we are sorting by the field we are already sorted by, reverse the order
    // If it is a different field, put the order back to ascending
    if (this.isSortedBy(sortBy)) {
      this.changePage.emit(new PagedEvent(
        1,
        this.items.pageSize,
        sortBy,
        this.sortOrder === SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending));
    } else {
      this.changePage.emit(new PagedEvent(
        1,
        this.items.pageSize,
        sortBy,
        SortOrder.Ascending));
    }
  }

  doCsvExport() {
    this.exportCsv.emit();
  }
}
