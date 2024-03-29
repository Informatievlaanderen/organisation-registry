import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Params, Router} from '@angular/router';

import {AlertBuilder, AlertService} from 'core/alert';
import {required} from 'core/validation';

import {SelectItem} from 'shared/components/form/form-group-select';

import {UpdateOrganisationService} from 'services/organisations';

import {PurposeService} from 'services/purposes';
import {OidcService} from "core/auth";
import {AllowedOrganisationFields, OrganisationInfoService} from "services";
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'edit.template.html',
  styleUrls: ['edit.style.css']
})
export class OrganisationInfoEditComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public purposes: SelectItem[];
  public articles: SelectItem[];

  private organisationId: string;
  private readonly subscriptions: Subscription[] = new Array<Subscription>();
  private allowedOrganisationFieldsToUpdate: AllowedOrganisationFields;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationService: UpdateOrganisationService,
    private oidcService: OidcService,
    private purposeService: PurposeService,
    private alertService: AlertService,
    private store: OrganisationInfoService
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

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  ngOnInit() {
    this.form.disable();
    this.subscriptions.push(
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
                .build())));

    this.route.parent.parent.params.forEach((params: Params) => {
      this.organisationId = params['id'];

      this.subscriptions.push(
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
                .build())));
    });

    this.subscriptions.push(
      this.store.allowedOrganisationFieldsToUpdateChanged$
        .subscribe(x => {
          this.allowedOrganisationFieldsToUpdate = x;
        }));
  }

  edit() {
    this.form.disable();

    const update = this.getUpdateService(this.allowedOrganisationFieldsToUpdate);
    this.subscriptions.push(
      update
        .finally(() => this.enableForm())
        .subscribe(
          result => {
            if (result) {
              this.router.navigate(['./..'], {relativeTo: this.route}).then(r => {
                this.alertService.setAlert(
                  new AlertBuilder()
                    .success()
                    .withTitle('Organisatie bijgewerkt!')
                    .withMessage('Organisatie is succesvol bijgewerkt.')
                    .build());

              });
            }
          },
          error => this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Organisatie kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build())));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private enableForm() {
    this.form.enable();
    this.form.get('ovoNumber').disable();
    if (this.form.value.kboNumber) {
      this.disableKboFields();
    }
    switch (this.allowedOrganisationFieldsToUpdate) {
      case AllowedOrganisationFields.None:
        this.disableVlimpersFields();
        this.disableNonVlimpersFields();
        break;
      case AllowedOrganisationFields.All:
        break;
      case AllowedOrganisationFields.OnlyVlimpers:
        this.disableNonVlimpersFields();
        break;
      case AllowedOrganisationFields.AllButVlimpers:
        this.disableVlimpersFields();
        break;
    }
  }

  private disableKboFields() {
    this.form.get('name').disable();
    this.form.get('shortName').disable();
  }

  private disableVlimpersFields() {
    this.form.get('name').disable();
    this.form.get('article').disable();
    this.form.get('shortName').disable();
    this.form.get('validFrom').disable();
    this.form.get('validTo').disable();
    this.form.get('operationalValidFrom').disable();
    this.form.get('operationalValidTo').disable();
  }

  private disableNonVlimpersFields() {
    this.form.get('description').disable();
    this.form.get('purposeIds').disable();
    this.form.get('showOnVlaamseOverheidSites').disable();
  }

  private getUpdateService(allowedOrganisationFieldsToUpdate: AllowedOrganisationFields) {
    switch (allowedOrganisationFieldsToUpdate) {
      case AllowedOrganisationFields.All:
        return this.organisationService.update(this.organisationId, this.form.value);
      case AllowedOrganisationFields.OnlyVlimpers:
        return this.organisationService.updateInfoLimitedToVlimpers(this.organisationId, this.form.value)
      case AllowedOrganisationFields.AllButVlimpers:
        return this.organisationService.updateInfoNotLimitedToVlimpers(this.organisationId, this.form.value);
      default:
        console.error('User not allowed to update any fields')
        break;
    }
  }
}
