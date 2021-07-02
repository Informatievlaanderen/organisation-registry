import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateOrganisationRegulationRequest,
  OrganisationRegulationService
} from 'services/organisationregulations';

import { RegulationType, RegulationTypeService } from 'services/regulationtypes';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationRegulationsCreateOrganisationRegulationComponent implements OnInit {
  public form: FormGroup;
  public regulationTypes: SelectItem[];

  private readonly createAlerts = new CreateAlertMessages('Regulation');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private regulationTypeService: RegulationTypeService,
    private organisationRegulationService: OrganisationRegulationService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationRegulationId: ['', required],
      organisationId: ['', required],
      regulationTypeId: [''],
      link: [''],
      date: [''],
      description: [''],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.form.disable();

    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateOrganisationRegulationRequest(params['id']));
    });

    this.regulationTypeService
      .getAllRegulationTypes()
      .finally(() => this.form.enable())
      .subscribe(
        allRegulationTypes => this.regulationTypes = allRegulationTypes.map(k => new SelectItem(k.id, k.name)),
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Regelgevingstypes konden niet geladen worden!')
              .withMessage('Er is een fout opgetreden bij het ophalen van de regelgevingstypes. Probeer het later opnieuw.')
              .build()));
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationRegulationRequest) {
    this.form.disable();

    this.organisationRegulationService.create(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Regelgeving aangemaakt!')
                .withMessage('Regelgeving is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Regelgeving kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build()));
  }
}
