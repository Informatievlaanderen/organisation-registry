import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { Role } from 'core/auth';

import { SelectItem } from 'shared/components/form/form-group-select';

import {
  CreateBodyOrganisationRequest,
  BodyOrganisationService
} from 'services/bodyorganisations';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class BodyOrganisationsCreateBodyOrganisationComponent implements OnInit {
  public form: FormGroup;
  public isOrgaanBeheerder: boolean;

  private readonly createAlerts = new CreateAlertMessages('Organisatie');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private bodyOrganisationService: BodyOrganisationService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      bodyOrganisationId: ['', required],
      bodyId: ['', required],
      organisationId: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    let roles = this.route.snapshot.data['userRoles'];
    this.isOrgaanBeheerder = roles.indexOf(Role.OrgaanBeheerder) !== -1;

    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateBodyOrganisationRequest(params['id']));
    });
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateBodyOrganisationRequest) {
    this.form.disable();

    this.bodyOrganisationService.create(value.bodyId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Organisatie gekoppeld!')
                .withMessage('Organisatie is succesvol gekoppeld.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Organisatie kon niet gekoppeld worden!')
              .withMessage('Er is een fout opgetreden bij het koppelen van de gegevens. Probeer het later opnieuw.')
              .build()));
  }
}
