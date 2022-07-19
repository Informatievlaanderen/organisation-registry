import { Response } from "@angular/http";
import { Injectable } from "@angular/core";
import { Headers, Http } from "@angular/http";

import { Observable } from "rxjs/Observable";

import { ConfigurationService } from "core/configuration";
import { HeadersBuilder } from "core/http";
import { PagedResult, PagedResultFactory, SortOrder } from "core/pagination";
import { ICrudService } from "core/crud";

import { OrganisationBankAccountListItem } from "./organisation-bank-account-list-item.model";
import { OrganisationBankAccount } from "./organisation-bank-account.model";
import { OrganisationBankAccountFilter } from "./organisation-bank-account-filter.model";

import {
  CreateOrganisationBankAccountRequest,
  UpdateOrganisationBankAccountRequest,
} from "./";

@Injectable()
export class OrganisationBankAccountService {
  constructor(
    private http: Http,
    private configurationService: ConfigurationService
  ) {}

  public getOrganisationBankAccounts(
    organisationId: string,
    filter: OrganisationBankAccountFilter,
    sortBy: string,
    sortOrder: SortOrder,
    page: number = 1,
    pageSize: number = this.configurationService.defaultPageSize
  ): Observable<PagedResult<OrganisationBankAccountListItem>> {
    let headers = new HeadersBuilder()
      .json()
      .withFiltering(filter)
      .withPagination(page, pageSize)
      .withSorting(sortBy, sortOrder)
      .build();

    return this.http
      .get(this.getOrganisationBankAccountsUrl(organisationId), {
        headers: headers,
      })
      .map(this.toOrganisationBankAccounts);
  }

  public get(
    organisationId: string,
    organisationBankAccountId: string
  ): Observable<OrganisationBankAccount> {
    const url = `${this.getOrganisationBankAccountsUrl(
      organisationId
    )}/${organisationBankAccountId}`;

    let headers = new HeadersBuilder().json().build();

    return this.http
      .get(url, { headers: headers })
      .map(this.toOrganisationBankAccount);
  }

  public create(
    organisationId,
    organisationBankAccount: CreateOrganisationBankAccountRequest
  ): Observable<string> {
    let headers = new HeadersBuilder().json().build();

    return this.http
      .post(
        this.getOrganisationBankAccountsUrl(organisationId),
        JSON.stringify(organisationBankAccount),
        { headers: headers }
      )
      .map(this.getBankAccountHeader);
  }

  public update(
    organisationId,
    organisationBankAccount: UpdateOrganisationBankAccountRequest
  ): Observable<boolean> {
    const url = `${this.getOrganisationBankAccountsUrl(organisationId)}/${
      organisationBankAccount.organisationBankAccountId
    }`;

    let headers = new HeadersBuilder().json().build();

    return this.http
      .put(url, JSON.stringify(organisationBankAccount), { headers: headers })
      .map((response) => response.ok);
  }

  private getOrganisationBankAccountsUrl(organisationId) {
    return `${this.configurationService.apiUrl}/v1/organisations/${organisationId}/bankAccounts`;
  }

  private getBankAccountHeader(res: Response): string {
    return res.headers.get("Location");
  }

  private toOrganisationBankAccount(res: Response): OrganisationBankAccount {
    let body = res.json();
    return body || ({} as OrganisationBankAccount);
  }

  private toOrganisationBankAccounts(
    res: Response
  ): PagedResult<OrganisationBankAccountListItem> {
    return new PagedResultFactory<OrganisationBankAccountListItem>().create(
      res.headers,
      res.json()
    );
  }

  delete(organisationId: string, organisationBankAccountId: string) {
    const url = `${this.getOrganisationBankAccountsUrl(
      organisationId
    )}/${organisationBankAccountId}`;
    let headers = new HeadersBuilder().json().build();

    return this.http.delete(url, { headers: headers });
  }
}
