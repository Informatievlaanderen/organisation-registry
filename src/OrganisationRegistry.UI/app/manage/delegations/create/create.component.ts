import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validator, Validators } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { CreateAlertMessages } from 'core/alertmessages';
import { Create, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { Person, PersonService } from 'services/people';
import { ContactTypeListItem } from 'services/contacttypes';

import {
  CreateDelegationAssignmentRequest,
  DelegationAssignmentService
} from 'services/delegationassignments';

import {
  Delegation,
  DelegationService
} from 'services/delegations';

@Component({
  templateUrl: 'create.template.html',
  styleUrls: ['create.style.css']
})
export class DelegationAssignimentsCreateDelegationAssignmentComponent implements OnInit {
  public form: FormGroup;
  public contactTypes: ContactTypeListItem[];
  public delegation: Delegation;

  private readonly createAlerts = new CreateAlertMessages('Toewijzing');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private personService: PersonService,
    private delegationService: DelegationService,
    private delegationAssignmentService: DelegationAssignmentService,
    private alertService: AlertService
  ) {
    this.form = formBuilder.group({
      delegationAssignmentId: ['', required],
      bodyId: ['', required],
      bodySeatId: ['', required],
      bodyMandateId: ['', required],
      personId: ['', required],
      validFrom: [''],
      validTo: [''],
    });

    this.delegation = new Delegation();
  }

  ngOnInit() {
    this.contactTypes = this.route.snapshot.data['contactTypes'];
    this.form.addControl('contacts', this.toFormGroup(this.contactTypes));

    this.route.params.forEach((params: Params) => {
      this.form.disable();
      let bodyMandateId = params['id'];
      let initialValue = new CreateDelegationAssignmentRequest(bodyMandateId);

      this.contactTypes.forEach(contactType => {
        initialValue.contacts[contactType.id] = '';
      });

      this.delegationService
        .get(bodyMandateId)
        .finally(() => this.form.enable())
        .subscribe(
          delegation => {
            this.delegation = delegation;
            initialValue.bodyId = delegation.bodyId;
            initialValue.bodySeatId = delegation.bodySeatId;
            this.form.setValue(initialValue);
          },
          error => this.handleError(error)
        );
    });
  }

  toFormGroup(contactTypes: ContactTypeListItem[]) {
    let group: any = {};

    contactTypes.forEach(contactType => {
      group[contactType.id] = new FormControl('');
    });

    return new FormGroup(group);
  }

  get isFormValid() {
    return this.form.enabled && this.form.valid;
  }

  create(value: CreateDelegationAssignmentRequest) {
    this.form.disable();

    this.delegationAssignmentService.create(value.bodyMandateId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./..'], { relativeTo: this.route });

            this.alertService.setAlert(
              new AlertBuilder()
                .success()
                .withTitle('Toewijzing aangemaakt!')
                .withMessage('Toewijzing is succesvol aangemaakt.')
                .build());
          }
        },
        error =>
          this.alertService.setAlert(
            new AlertBuilder()
              .error(error)
              .withTitle('Toewijzing kon niet bewaard worden!')
              .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
              .build()));
  }

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Delegatie kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de delegatie. Probeer het later opnieuw.')
        .build()
    );
  }
}
