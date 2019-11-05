import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateOrganisationFormalFrameworkRequest,
  OrganisationFormalFrameworkService
} from 'services/organisationformalframeworks';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationFormalFrameworksCreateOrganisationFormalFrameworkComponent implements OnInit {
  public form: FormGroup;

  private readonly createAlerts = new CreateAlertMessages('Toepassingsgebied');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationFormalFrameworkService: OrganisationFormalFrameworkService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationFormalFrameworkId: ['', required],
      organisationId: ['', required],
      formalFrameworkId: ['', required],
      parentOrganisationId: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateOrganisationFormalFrameworkRequest(params['id']));
    });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationFormalFrameworkRequest) {
    this.form.disable();

    this.organisationFormalFrameworkService.create(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Toepassingsgebied gekoppeld!')
                .withMessage('Toepassingsgebied is succesvol gekoppeld.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Toepassingsgebied kon niet gekoppeld worden!')
              .withMessage('Er is een fout opgetreden bij het koppelen van de gegevens. Probeer het later opnieuw.')
              .build()));
  }
}
