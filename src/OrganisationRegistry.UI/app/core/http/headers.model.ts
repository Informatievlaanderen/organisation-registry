import { Headers } from "@angular/http";
import { SortOrder } from "./../pagination";

export class HeadersBuilder {
  private _headers = new Headers();

  private _title: string;
  private _message: string;
  private _link: string;

  public json(): HeadersBuilder {
    this._headers.append("accept", "application/json");
    return this;
  }

  public contentJson(): HeadersBuilder {
    this._headers.append("Content-Type", "application/json");
    return this;
  }

  public csv(): HeadersBuilder {
    this._headers.append("accept", "text/csv");
    return this;
  }

  public withFiltering(filter) {
    this._headers.append(
      "x-filtering",
      encodeURIComponent(JSON.stringify(filter))
    );
    return this;
  }

  public withPagination(page: number, pageSize: number) {
    this._headers.append("x-pagination", `${page},${pageSize}`);
    return this;
  }

  public withoutPagination() {
    this._headers.append("x-pagination", "none");
    return this;
  }

  public withSorting(sortBy: string, sortOrder: SortOrder) {
    this._headers.append(
      "x-sorting",
      `${SortOrder[sortOrder].toLowerCase()},${sortBy}`
    );
    return this;
  }

  public build(token: string = null): Headers {
    if (!token) token = localStorage.getItem("token");
    if (!!token) this._headers.append("Authorization", `Bearer ${token}`);

    return this._headers;
  }
}
