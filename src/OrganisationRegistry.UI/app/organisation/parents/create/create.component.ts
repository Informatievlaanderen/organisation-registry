import {Component, OnDestroy, OnInit} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import {
  CreateOrganisationParentRequest,
  OrganisationParentService
} from 'services/organisationparents';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class OrganisationParentsCreateOrganisationParentComponent implements OnInit, OnDestroy {
  public form: FormGroup;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private organisationParentService: OrganisationParentService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      organisationOrganisationParentId: ['', required],
      organisationId: ['', required],
      parentOrganisationId: ['', required],
      validFrom: [''],
      validTo: [''],
    });
  }

  ngOnInit() {
    this.route.parent.parent.params.forEach((params: Params) => {
      this.form.setValue(new CreateOrganisationParentRequest(params['id']));
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateOrganisationParentRequest) {
    this.form.disable();

    this.subscriptions.push(this.organisationParentService.create(value.organisationId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], {relativeTo: this.route});

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Moeder entiteit gekoppeld!')
                .withMessage('Functie is succesvol gekoppeld.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Moeder entiteit kon niet gekoppeld worden!')
              .withMessage('Er is een fout opgetreden bij het koppelen van de gegevens. Probeer het later opnieuw.')
              .build())));
  }
}
