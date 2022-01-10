import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Params, Router} from '@angular/router';

import {AlertBuilder, AlertService} from 'core/alert';
import {required} from 'core/validation';

import {SelectItem} from 'shared/components/form/form-group-select';

import {UpdateOrganisationService} from 'services/organisations';

import {PurposeService} from 'services/purposes';
import {OidcService, Role} from "../../../core/auth";

@Component({
  templateUrl: 'edit.template.html',
  styleUrls: ['edit.style.css']
})
export class OrganisationInfoEditComponent implements OnInit {
  public form: FormGroup;
  public purposes: SelectItem[];
  public articles: SelectItem[];

  private organisationId: string;
  private underVlimpersManagement: boolean;
  private isVlimpersBeheerder: boolean;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationService: UpdateOrganisationService,
    private oidcService: OidcService,
    private purposeService: PurposeService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      id: ['', required],
      name: ['', required],
      ovoNumber: ['', required],
      kboNumber: [''],
      shortName: [''],
      article: [''],
      description: ['', Validators.nullValidator],
      formalFrameworkId: ['', Validators.nullValidator],
      organisationClassificationId: ['', Validators.nullValidator],
      organisationClassificationTypeId: ['', Validators.nullValidator],
      purposeIds: [[]],
      purposes: [[]],
      parentOrganisationId: [''],
      parentOrganisation: [''],
      showOnVlaamseOverheidSites: [false],
      validFrom: [''],
      validTo: [''],
      operationalValidFrom: [''],
      operationalValidTo: [''],
      isTerminated: [false],
      underVlimpersManagement: [false]
    });
    this.articles = [
      new SelectItem('de', 'de'),
      new SelectItem('het', 'het'),
    ]
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

    this.oidcService.isVlimpersBeheerder()
      .subscribe(isVlimpersBeheerder => this.isVlimpersBeheerder = isVlimpersBeheerder);
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  edit() {
    this.form.disable();

    this.organisationService.update(this.organisationId, this.form.value)
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
    if (this.form.value.underVlimpersManagement && !this.isVlimpersBeheerder) {
      this.form.get('name').disable();
      this.form.get('article').disable();
      this.form.get('shortName').disable();
      this.form.get('validFrom').disable();
      this.form.get('validTo').disable();
      this.form.get('operationalValidFrom').disable();
      this.form.get('operationalValidTo').disable();
    }
  }
}
