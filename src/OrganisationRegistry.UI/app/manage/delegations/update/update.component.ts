import { Component, OnInit, ViewChild, ElementRef, Renderer } from '@angular/core';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';
import { ActivatedRoute, Router, Params } from '@angular/router';

import { Observable } from 'rxjs/Observable';

import { AlertService, AlertBuilder, Alert, AlertType } from 'core/alert';
import { UpdateAlertMessages } from 'core/alertmessages';
import { Update, ICrud } from 'core/crud';
import { required } from 'core/validation';

import { SearchResult } from 'shared/components/form/form-group-autocomplete';

import { Person, PersonService } from 'services/people';
import { ContactTypeListItem } from 'services/contacttypes';

import {
  UpdateDelegationAssignmentRequest,
  DelegationAssignmentService
} from 'services/delegationassignments';

import {
  Delegation,
  DelegationService
} from 'services/delegations';

@Component({
  templateUrl: 'update.template.html',
  styleUrls: ['update.style.css']
})
export class DelegationAssignimentsUpdateDelegationAssignmentComponent implements OnInit {
  public form: FormGroup;
  public person: SearchResult;
  public delegation: Delegation;
  public contactTypes: ContactTypeListItem[];

  private readonly updateAlerts = new UpdateAlertMessages('Toewijzing');

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private formBuilder: FormBuilder,
    private personService: PersonService,
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
      personId: ['', required],
      personName: ['', required],
      validFrom: [''],
      validTo: ['']
    });

    this.delegation = new Delegation();
  }

  ngOnInit() {
    this.contactTypes = this.route.snapshot.data['contactTypes'];
    this.form.addControl('contacts', this.toFormGroup(this.contactTypes));

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
              this.setForm(res[1]);
            },
            error => this.handleError(error));
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

  update(value: UpdateDelegationAssignmentRequest) {
    this.form.disable();

    this.delegationAssignmentService.update(value.bodyMandateId, value)
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

  private setForm(delegationAssignment) {
    if (delegationAssignment) {
      if (!delegationAssignment.contacts)
        delegationAssignment.contacts = {};

      this.contactTypes.forEach(contactType => {
        if (!delegationAssignment.contacts[contactType.id])
          delegationAssignment.contacts[contactType.id] = '';
      });

      this.form.setValue(delegationAssignment);
      this.person = new SearchResult(delegationAssignment.personId, delegationAssignment.personName);
    }

    this.form.enable();
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
        .withTitle('Toewijzing bijgewerkt!')
        .withMessage('Toewijzing is succesvol bijgewerkt.')
        .build());
  }

  private handleSaveError(error) {
    this.alertService.setAlert(
      new AlertBuilder()
        .error(error)
        .withTitle('Toewijzing kon niet bewaard worden!')
        .withMessage('Er is een fout opgetreden bij het bewaren van de gegevens. Probeer het later opnieuw.')
        .build());
  }
}
