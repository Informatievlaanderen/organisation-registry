import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';

import { Alert, AlertBuilder, AlertService, AlertType } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { ICrud, Update } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';
import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import {
  UpdateOrganisationBuildingRequest,
  OrganisationBuildingService
} from 'services/organisationbuildings';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class OrganisationBuildingsUpdateOrganisationBuildingComponent implements OnInit {
  public form: FormGroup;
  public building: SearchResult;

  private readonly updateAlerts = new UpdateAlertMessages('Gebouw');
  private buildingId: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationBuildingService: OrganisationBuildingService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationBuildingId: ['', required],
      organisationId: ['', required],
      buildingId: ['', required],
      buildingName: ['', required],
      isMainBuilding: [false, Validators.required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    Observable.zip(this.route.parent.parent.params, this.route.params)
      .subscribe(res => {
        this.form.disable();
        let orgId = res[0]['id'];
        let organisationBuildingId = res[1]['id'];

        this.organisationBuildingService
          .get(orgId, organisationBuildingId)
          .subscribe(
            item => this.setForm(item),
            error => this.handleError(error));
      });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  update(value: UpdateOrganisationBuildingRequest) {
    this.form.disable();

    this.organisationBuildingService.update(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
      result => {
          if (result) {
            this.router.navigate(['./../..'], { relativeTo: this.route });
            this.handleSaveSuccess();
          }
        },
        error => this.handleSaveError(error)
      );
  }

  private setForm(organisationBuilding) {
    if (organisationBuilding) {
      this.form.setValue(organisationBuilding);
      this.building = new SearchResult(
              organisationBuilding.buildingId,
              organisationBuilding.buildingName);
    }

    this.form.enable();
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Gebouw kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van het gebouw. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Gebouw bijgewerkt!')
        .withMessage('Gebouw is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    // TODO: Show the ApiProblem alert if there is one
    // (there should always be one except if the problem did not originate in the API)
    // and otherwise show this alert.
    // NOTE: Maybe we should do this only if we have a DomainException. (But how to tell? Empty details? Extra property on ApiProblem?)

    // this.alertService.setAlert(
    //   new AlertBuilder()
    //     .error(error)
    //     .withTitle('Gebouw kon niet bewaard worden!')
    //     .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
    //     .build());
  }
}
