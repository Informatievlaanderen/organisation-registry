import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, Alert, AlertType, AlertBuilder } from 'core/alert';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SearchResult } from 'shared/components/form/form-group-autocomplete';
import { SelectItem } from 'shared/components/form/form-group-select';

import {
  UpdateOrganisationRequest,
  UpdateOrganisationService
} from 'services/organisations';

import { Purpose, PurposeService } from 'services/purposes';

@Component({
  templateUrl: 'edit.template.html',
  styleUrls: ['edit.style.css']
})
export class OrganisationInfoEditComponent implements OnInit {
  public form: FormGroup;
  public purposes: SelectItem[];

  private organisationId: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationService: UpdateOrganisationService,
    private purposeService: PurposeService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      id: ['', required],
      name: ['', required],
      ovoNumber: ['', required],
      kboNumber: [''],
      shortName: [''],
      description: ['', Validators.nullValidator],
      formalFrameworkId: ['', Validators.nullValidator],
      organisationClassificationId: ['', Validators.nullValidator],
      organisationClassificationTypeId: ['', Validators.nullValidator],
      purposeIds: [[]],
      purposes: [[]],
      parentOrganisationId: [''],
      parentOrganisation: [''],
      mainBuildingId: [''],
      mainBuildingName: [''],
      mainLocationId: [''], // this is BS, man...
      mainLocationName: [''],
      showOnVlaamseOverheidSites: [false],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.form.disable();
    this.purposeService
      .getAllPurposes()
      .finally(() => this.enableForm())
      .subscribe(
        allPurposes => this.purposes = allPurposes.map(k => new SelectItem(k.id, k.name)),
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Beleidsvelden konden niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de beleidsvelden. Probeer het later opnieuw.')
              .build()));

    this.route.parent.parent.params.forEach((params: Params) => {
      this.organisationId = params['id'];

      this.organisationService.get(this.organisationId)
        .finally(() => this.enableForm())
        .subscribe(
          item => {
            if (item) {
              this.form.setValue(item);
            }
          },
          error => this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Organisatie kon niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de organisatie. Probeer het later opnieuw.')
              .build()));
    });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  edit(value: UpdateOrganisationRequest) {
    this.form.disable();

    this.organisationService.update(this.organisationId, value)
      .finally(() => this.enableForm())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Organisatie bijgewerkt!')
                .withMessage('Organisatie is succesvol bijgewerkt.')
                .build());
          }
        },
        error => this.alertService.setAlert(
          new AlertBuilder()
            .error(error)
            .withTitle('Organisatie kon niet bewaard worden!')
            .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
            .build()));
  }

  private enableForm() {
    this.form.enable();
    this.form.get('ovoNumber').disable();
    if (this.form.value.kboNumber) {
      this.form.get('name').disable();
      this.form.get('shortName').disable();
    }
  }
}
