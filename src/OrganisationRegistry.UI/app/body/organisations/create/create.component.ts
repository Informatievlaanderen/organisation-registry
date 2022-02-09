import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import { Role } from 'core/auth';

import {
  CreateBodyOrganisationRequest,
  BodyOrganisationService
} from 'services/bodyorganisations';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class BodyOrganisationsCreateBodyOrganisationComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public isOrgaanBeheerder: boolean;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

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

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateBodyOrganisationRequest) {
    this.form.disable();

    this.subscriptions.push(this.bodyOrganisationService.create(value.bodyId, value)
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
              .build())));
  }
}
