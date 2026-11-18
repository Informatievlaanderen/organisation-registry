import { Component, Input, Output, EventEmitter } from "@angular/core";
import { Observable } from "rxjs/Observable";

import { BaseListComponent } from "shared/components/list";
import { OidcService } from "core/auth";
import { OrganisationCapacityListItem } from "services/organisationcapacities";

@Component({
  selector: "ww-organisation-capacity-list",
  templateUrl: "list.template.html",
  styleUrls: ["list.style.css"],
  inputs: ["items", "isBusy"],
  outputs: ["changePage"],
})
export class OrganisationCapacitiesListComponent extends BaseListComponent<OrganisationCapacityListItem> {
  isLoggedIn: Observable<boolean>;

  constructor(oidcService: OidcService) {
    super();
    this.isLoggedIn = oidcService.isLoggedIn;
  }

  @Input("canEdit") canEdit: Observable<boolean>;
  @Input("canDelete") canDelete: Observable<boolean>;
  @Output()
  removeCapacityClicked: EventEmitter<OrganisationCapacityListItem> = new EventEmitter<OrganisationCapacityListItem>();

  remove(item: OrganisationCapacityListItem) {
    this.removeCapacityClicked.emit(item);
  }
}
