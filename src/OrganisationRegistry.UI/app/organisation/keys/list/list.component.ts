import { Component, Input, Output, EventEmitter } from "@angular/core";
import { Observable } from "rxjs/Observable";

import { BaseListComponent } from "shared/components/list";
import { OrganisationKeyListItem } from "services/organisationkeys";

@Component({
  selector: "ww-organisation-key-list",
  templateUrl: "list.template.html",
  styleUrls: ["list.style.css"],
  inputs: ["items", "isBusy"],
  outputs: ["changePage"],
})
export class OrganisationKeysListComponent extends BaseListComponent<OrganisationKeyListItem> {
  @Input("canEdit") canEdit: Observable<boolean>;
}
