import {Component, OnDestroy, OnInit, Renderer} from '@angular/core';
import { FormBuilder, FormGroup} from '@angular/forms';
import { ActivatedRoute, Router} from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder} from 'core/alert';
import { required } from 'core/validation';

import {
  DeleteDelegationAssignmentRequest,
  DelegationAssignmentService,
  DelegationAssignment
} from 'services/delegationassignments';

import {
  Delegation,
  DelegationService
} from 'services/delegations';
import {Subscription} from "rxjs/Subscription";

@Component({
  templateUrl: 'delete.template.html',
  styleUrls: ['delete.style.css']
})
export class DelegationAssignimentsDeleteDelegationAssignmentComponent implements OnInit, OnDestroy {
  public form: FormGroup;
  public delegation: Delegation;
  public delegationAssignment: DelegationAssignment;

  private readonly subscriptions: Subscription[] = new Array<Subscription>();

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
    this.subscriptions.push(this.route.params
      .subscribe(res => {
        this.form.disable();
        let bodyMandateId = res['bodyMandateId'];
        let delegationAssignmentId = res['id'];

        let delegation$ =
          this.delegationService.get(bodyMandateId);

        let delegationAssignment$ =
          this.delegationAssignmentService.get(bodyMandateId, delegationAssignmentId);

        this.subscriptions.push(Observable.zip(delegation$, delegationAssignment$)
          .finally(() => this.form.enable())
          .subscribe(
            res => {
              this.delegation = res[0];
              this.delegationAssignment = res[1];
              this.form.setValue(this.delegationAssignment);
            },
            error => this.handleError(error)));
      }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  delete(value: DeleteDelegationAssignmentRequest) {
    this.form.disable();

    this.subscriptions.push(this.delegationAssignmentService.delete(value.bodyMandateId, value)
      .finally(() => this.form.enable())
      .subscribe(
        result => {
          if (result) {
            this.router.navigate(['./../..'], { relativeTo: this.route });
            this.handleSaveSuccess();
          }
        },
        error => this.handleSaveError(error)
      ));
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
