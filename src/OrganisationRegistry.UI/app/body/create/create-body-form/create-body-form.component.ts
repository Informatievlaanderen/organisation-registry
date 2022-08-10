import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  OnInit,
  Output,
} from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";

import { required } from "core/validation";
import { Role } from "core/auth";

import { CreateBodyFormValues } from "./create-body-form.model";

@Component({
  selector: "ww-create-body-form",
  templateUrl: "create-body-form.template.html",
  styleUrls: ["create-body-form.style.css"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CreateBodyFormComponent implements OnInit {
  public isOrgaanBeheerder: boolean = false;

  @Input("cancelRouterLink") cancelRouterLink;
  @Input("primaryButtonText") primaryButtonText;
  @Input("body") body;
  @Input("initialOrganisation") initialOrganisation;

  private _isBusy: boolean;
  @Input("isBusy")
  public set isBusy(value: boolean) {
    this._isBusy = value;
    if (this.form) this.updateForm();
  }

  private updateForm() {
    if (this._isBusy) {
      this.form.disable();
    } else {
      this.form.enable();
      if (this.initialOrganisation) {
        this.form.get("organisationId").disable();
      }
    }
  }

  @Output("onSubmit") onSubmit: EventEmitter<CreateBodyFormValues> =
    new EventEmitter<CreateBodyFormValues>();

  public form: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit() {
    let roles = this.route.snapshot.data["userRoles"];
    let isOrganisatieBeheerder = roles.indexOf(Role.DecentraalBeheerder) !== -1;
    this.isOrgaanBeheerder = roles.indexOf(Role.OrgaanBeheerder) !== -1;

    this.form = this.formBuilder.group({
      id: ["", required],
      bodyNumber: [""],
      name: ["", required],
      shortName: ["", Validators.nullValidator],
      description: ["", Validators.nullValidator],
      validFrom: [""],
      validTo: [""],
      formalValidFrom: [""],
      formalValidTo: [""],
      organisationId: [
        "",
        isOrganisatieBeheerder ? required : Validators.nullValidator,
      ],
    });
    this.form.setValue(this.body);
    this.updateForm();
  }

  submit(value: CreateBodyFormValues) {
    if (this.initialOrganisation) {
      value.organisationId = this.initialOrganisation.value;
    }
    this.onSubmit.next(value);
  }
}
