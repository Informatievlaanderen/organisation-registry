import { Component, OnInit, ViewChild, ElementRef, Renderer } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { Update, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { ContactTypeListItem } from 'services/contacttypes';

import {
  DeleteDelegationAssignmentRequest,
  DelegationAssignmentService,
  DelegationAssignment
} from 'services/delegationassignments';

import {
  Delegation,
  DelegationService
} from 'services/delegations';

@Component({
  templateUrl: 'delete.template.html',
  styleUrls: ['delete.style.css']
})
export class DelegationAssignimentsDeleteDelegationAssignmentComponent implements OnInit {
  public form: FormGroup;
  public delegation: Delegation;
  public delegationAssignment: DelegationAssignment;

  private readonly updateAlerts = new UpdateAlertMessages('Toewijzing');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private delegationService: DelegationService,
    private delegationAssignmentService: DelegationAssignmentService,
    private alertService: AlertService,
    private renderer: Renderer
  ) {
    this.form = formBuilder.group({
      delegationAssignmentId: ['', required],
      bodyId: ['', required],
      bodySeatId: ['', required],
      bodyMandateId: ['', required],

      personId: [''],
      personName: [''],
      validFrom: [''],
      validTo: [''],
      contacts: ['']
    });

    this.delegation = new Delegation();
    this.delegationAssignment = new DelegationAssignment();
  }

  ngOnInit() {
    this.route.params
      .subscribe(res => {
        this.form.disable();
        let bodyMandateId = res['bodyMandateId'];
        let delegationAssignmentId = res['id'];

        let delegation$ =
          this.delegationService.get(bodyMandateId);

        let delegationAssignment$ =
          this.delegationAssignmentService.get(bodyMandateId, delegationAssignmentId);

        Observable.zip(delegation$, delegationAssignment$)
          .finally(() => this.form.enable())
          .subscribe(
            res => {
              this.delegation = res[0];
              this.delegationAssignment = res[1];
              this.form.setValue(this.delegationAssignment);
            },
            error => this.handleError(error));
      });
  }

  // TODO: Rename to delete
  delete(value: DeleteDelegationAssignmentRequest) {
    this.form.disable();

    this.delegationAssignmentService.delete(value.bodyMandateId, value)
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

  private handleError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Delegatie kon niet geladen worden!')
        .withMessage('Er is een fout opgetreden bij het ophalen van de delegaties. Probeer het later opnieuw.')
        .build()
    );
  }

  private handleSaveSuccess() {
    this.alertService.setAlert(
      new AlertBuilder()
        .success()
        .withTitle('Toewijzing verwijderd!')
        .withMessage('Toewijzing is succesvol verwijderd.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Toewijzing kon niet verwijderd worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
