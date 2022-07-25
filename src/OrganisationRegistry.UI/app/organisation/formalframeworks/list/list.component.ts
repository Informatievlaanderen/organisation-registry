import { Component, EventEmitter, Input, Output } from "@angular/core";

import { BaseListComponent } from "shared/components/list";
import { OrganisationFormalFrameworkListItem } from "services/organisationformalframeworks";
import { Observable } from "rxjs/Observable";
import { FormalFramework } from "../../../services";
import { OrganisationBankAccountListItem } from "../../../services/organisationbankaccounts";

@Component({
  selector: "ww-organisation-formal-framework-list",
  templateUrl: "list.template.html",
  styleUrls: ["list.style.css"],
  inputs: ["items", "isBusy"],
  outputs: ["changePage"],
})
export class OrganisationFormalFrameworksListComponent extends BaseListComponent<OrganisationFormalFrameworkListItem> {
  @Input("canEdit") canEdit: Observable<boolean>;
  @Input("canDelete") canDelete: Observable<boolean>;

  @Output()
  removeOrganisationFormalFrameworkClicked: EventEmitter<OrganisationFormalFrameworkListItem> =
    new EventEmitter<OrganisationFormalFrameworkListItem>();

  remove(formalFramework: OrganisationFormalFrameworkListItem) {
    this.removeOrganisationFormalFrameworkClicked.emit(formalFramework);
  }
}
