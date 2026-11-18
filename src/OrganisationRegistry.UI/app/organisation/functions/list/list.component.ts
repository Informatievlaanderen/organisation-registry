import { Component, Input, Output, EventEmitter } from "@angular/core";
import { Observable } from "rxjs/Observable";

import { BaseListComponent } from "shared/components/list";
import { OidcService } from "core/auth";
import { OrganisationFunctionListItem } from "services/organisationfunctions";
import { OrganisationBankAccountListItem } from "../../../services/organisationbankaccounts";

@Component({
  selector: "ww-organisation-function-list",
  templateUrl: "list.template.html",
  styleUrls: ["list.style.css"],
  inputs: ["items", "isBusy"],
  outputs: ["changePage"],
})
export class OrganisationFunctionsListComponent extends BaseListComponent<OrganisationFunctionListItem> {
  isLoggedIn: Observable<boolean>;

  constructor(oidcService: OidcService) {
    super();
    this.isLoggedIn = oidcService.isLoggedIn;
  }

  @Input("canEdit") canEdit: Observable<boolean>;
  @Input("canDelete") canDelete: Observable<boolean>;
  @Output()
  removeClicked: EventEmitter<OrganisationFunctionListItem> = new EventEmitter<OrganisationFunctionListItem>();
}
